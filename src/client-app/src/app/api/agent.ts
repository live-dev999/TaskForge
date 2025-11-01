import axios, { AxiosResponse } from 'axios';
import { TaskItem } from '../models/taskItem';

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

axios.interceptors.response.use(async respose => {
    try {
        await sleep(1000);
        return respose;
    } catch (error) {
        console.log(error);
        return await Promise.reject(error);
    }
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
    create: (taskItem: TaskItem) => requests.post<void>('/taskItems', taskItem),
    update: (taskItem: TaskItem) => requests.put<void>(`/taskItems/${taskItem.id}`, taskItem),
    delete: (id: string) => requests.del(`/taskItems/${id}`),
}

const agent = {
    TaskItems
}

export default agent;