import axios, { AxiosResponse } from 'axios';
import { TaskItem } from '../models/taskItem';

const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay)
    })
}
axios.defaults.baseURL = "http://localhost:5000/api"

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