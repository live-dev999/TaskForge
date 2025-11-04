import axios, { AxiosError, AxiosResponse } from 'axios';
import { TaskItem } from '../models/taskItem';
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
    return window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1'
        ? "http://localhost:5009/api"
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
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const TaskItems = {
    list: () => requests.get<TaskItem[]>('/taskItems'),
    details: (id: string) => requests.get<TaskItem>(`/taskItems/${id}`),
    create: (taskItem: TaskItem) => requests.post<TaskItem>('/taskItems', taskItem),
    update: (taskItem: TaskItem) => requests.put<void>(`/taskItems/${taskItem.id}`, taskItem),
    delete: (id: string) => requests.del(`/taskItems/${id}`),
}

const agent = {
    TaskItems
}

export default agent;