import axios, { AxiosError, AxiosResponse } from 'axios';
import { TaskItem } from '../models/taskItem';
import { Pagination, PaginatedResult } from '../models/pagination';
import { toast } from 'react-toastify';
import { router } from '../router/Routes';
import { store } from '../stores/store';

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay)
    })
}
// Use environment variable for API URL
// In Docker, use relative path to leverage nginx proxy
// For local development without Docker, use full URL
const getApiBaseURL = () => {
    const envUrl = process.env.REACT_APP_API_URL;
    if (envUrl) {
        return envUrl;
    }
    // If no env var, check if we're in Docker (relative path will work with nginx proxy)
    // For local development, use full URL
    // API runs on port 5000 (HTTP) or 5263 (HTTPS) based on launchSettings.json
    return window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1'
        ? "http://localhost:5000/api"
        : "/api"; // Relative path for Docker/production (nginx will proxy)
};

axios.defaults.baseURL = getApiBaseURL();

// Log base URL for debugging
console.log('API Base URL:', axios.defaults.baseURL);

axios.interceptors.response.use(async response => {
    //await sleep(1000);
    return response;
}, (error: AxiosError) => {
    if (error.response) {
        const { data, status, config } = error.response as AxiosResponse;
        switch (status) {
            case 400:
                if (config.method === 'get' && data && typeof data === 'object' && Object.prototype.hasOwnProperty.call(data, 'errors') && Object.prototype.hasOwnProperty.call(data.errors, 'id')) {
                    router.navigate('/not-found');
                }
                //toast.error('bad request')
                if (data && typeof data === 'object' && 'errors' in data && data.errors) {
                    const modalStateErrors = [];
                    for (const key in data.errors) {
                        if (data.errors[key])
                            modalStateErrors.push(data.errors[key])
                    }
                    throw modalStateErrors.flat();
                } else {
                    toast.error(typeof data === 'string' ? data : 'Bad request');
                }
                break;
            case 401:
                toast.error('unauthorised')
                break;
            case 403:
                toast.error('forbidden')
                break;
            case 404:
                //toast.error('not found')
                router.navigate('/not-found');
                break;
            case 500:
                store.commonStore.setServerError(data);
                router.navigate('/server-error');
                //toast.error('server error ')
                break;
        }
    } else if (error.request) {
        // Request was made but no response received
        toast.error('Server is not responding. Please check if the API is running.');
        console.error('Request error:', error.message);
    } else {
        // Something else happened
        toast.error('An unexpected error occurred');
        console.error('Error:', error.message);
    }
    return Promise.reject(error);
})

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T>(url: string, params?: URLSearchParams) => {
        const config = params ? { params } : {};
        return axios.get<T>(url, config).then(responseBody);
    },
    getWithHeaders: <T>(url: string, params?: URLSearchParams) => {
        const config = params ? { params } : {};
        return axios.get<T>(url, config);
    },
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

// Note: Backend sends pagination data in HTTP headers, not in the response body
// The response body is just an array of TaskItems

const TaskItems = {
    list: (params?: URLSearchParams) => 
        requests.getWithHeaders<TaskItem[]>('/TaskItems', params).then(response => {
            // Extract array items from response
            const items = response.data as TaskItem[];
            
            // Pagination data is always in HTTP headers (backend sends it in "Pagination" header)
            // Axios normalizes headers to lowercase, but check both cases to be safe
            const headerKey = Object.keys(response.headers).find(key => key.toLowerCase() === 'pagination');
            const paginationHeader = headerKey ? response.headers[headerKey] : null;
            
            let pagination: Pagination;
            
            if (paginationHeader) {
                try {
                    const headerData = typeof paginationHeader === 'string' 
                        ? JSON.parse(paginationHeader) 
                        : paginationHeader;
                    
                    pagination = {
                        currentPage: headerData.currentPage,
                        itemsPerPage: headerData.itemsPerPage,
                        totalItems: headerData.totalItems,
                        totalPages: headerData.totalPages
                    };
                } catch (e) {
                    console.error('Failed to parse pagination header:', e, paginationHeader);
                    // Fallback: create pagination from items
                    pagination = {
                        currentPage: 1,
                        itemsPerPage: 10,
                        totalItems: items.length,
                        totalPages: 1
                    };
                }
            } else {
                console.warn('No pagination header found in response');
                // Fallback: create pagination from items
                pagination = {
                    currentPage: 1,
                    itemsPerPage: 10,
                    totalItems: items.length,
                    totalPages: 1
                };
            }
            
            // Log for debugging
            console.log('PagedList response:', {
                itemsCount: items.length,
                pagination,
                paginationHeader: paginationHeader
            });
            
            return { items, pagination } as PaginatedResult<TaskItem[]>;
        }),
    details: (id: string) => requests.get<TaskItem>(`/TaskItems/${id}`),
    create: (taskItem: TaskItem) => requests.post<TaskItem>('/TaskItems', taskItem),
    update: (taskItem: TaskItem) => requests.put<void>(`/TaskItems/${taskItem.id}`, taskItem),
    delete: (id: string) => requests.del(`/TaskItems/${id}`),
}

const agent = {
    TaskItems
}

export default agent;