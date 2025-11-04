import { makeAutoObservable, runInAction } from "mobx"
import agent from "../api/agent";
import { TaskItem } from "../models/taskItem"

import { v4 as uuid } from 'uuid';
import { Pagination } from "../models/pagination";

export default class TaskItemStore {
    taskItemRegistry = new Map<string, TaskItem>();
    selectedTaskItem: TaskItem | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = true;
    pagination: Pagination | null = null;

    constructor() {
        makeAutoObservable(this)
    }
    get taskItemsByDate() {
        return Array.from(this.taskItemRegistry.values()).sort((a, b) =>
            Date.parse(a.createdAt) - Date.parse(b.createdAt));
    }

    get groupedTaskItems() {
        return Object.entries(
            this.taskItemsByDate.reduce((taskItems, taskItem) => {
                const date = taskItem.createdAt;
                taskItems[date] = taskItems[date] ? [...taskItems[date], taskItem] : [taskItem];
                return taskItems;
            }, {} as { [key: string]: TaskItem[] })
        )
    }


    loadTaskItems = async (pageNumber: number = 1, pageSize: number = 10) => {
        this.setLoadingInitial(true);
        try {
            const params = new URLSearchParams();
            params.append('pageNumber', pageNumber.toString());
            params.append('pageSize', pageSize.toString());
            
            const result = await agent.TaskItems.list(params);

            runInAction(() => {
                this.taskItemRegistry.clear();
                result.items.forEach(taskItem => {
                    this.setTaskItem(taskItem);
                });
                this.pagination = result.pagination;
                this.setLoadingInitial(false);
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.setLoadingInitial(false);
            });
        }
    }

    setPagination = (pagination: Pagination | null) => {
        this.pagination = pagination;
    }

    loadTaskItem = async (id: string): Promise<TaskItem | undefined> => {
        let taskItem = this.getTaskItem(id)
        if (taskItem) {
            this.selectedTaskItem = taskItem;
            return taskItem;
        }
        else {
            this.setLoadingInitial(true);
            try {
                taskItem = await agent.TaskItems.details(id);
                this.setTaskItem(taskItem);
                runInAction(() => {
                    this.selectedTaskItem = taskItem;
                });
                this.setLoadingInitial(false)
                return taskItem;
            } catch (error) {
                console.error('Error loading task item:', error);
                this.setLoadingInitial(false);
                // If error is 404, navigation is handled by interceptor
                // Return undefined so form can handle it gracefully
                return undefined;
            }
        }
    }
    private setTaskItem = (taskItem: TaskItem) => {
        taskItem.createdAt = taskItem.createdAt.split('T')[0];
        taskItem.updatedAt = taskItem.updatedAt.split('T')[0];
        this.taskItemRegistry.set(taskItem.id, taskItem);
    }
    private getTaskItem = (id: string) => {
        return this.taskItemRegistry.get(id);
    }
    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    createTaskItem = async (taskItem: TaskItem): Promise<TaskItem | undefined> => {
        this.loading = true;
        // Don't modify taskItem.id here - form sends Guid.Empty (00000000-0000-0000-0000-000000000000)
        // Server will check if Id == Guid.Empty and generate new Guid if needed

        try {
            const createdTaskItem = await agent.TaskItems.create(taskItem);
            if (createdTaskItem) {
                runInAction(() => {
                    // Use the created task item from API (with correct ID, CreatedAt/UpdatedAt from server)
                    this.setTaskItem(createdTaskItem);
                    this.selectedTaskItem = createdTaskItem;
                    this.editMode = false;
                    this.loading = false;
                });
                return createdTaskItem;
            } else {
                runInAction(() => {
                    this.loading = false;
                });
                return undefined;
            }
        } catch (error) {
            console.error('Error creating task item:', error);
            runInAction(() => {
                this.loading = false;
            });
            throw error; // Re-throw to allow form to handle error
        }
    }

    updateTaskItem = async (taskItem: TaskItem) => {
        this.loading = true;

        try {
            await agent.TaskItems.update(taskItem)
            runInAction(() => {
                this.taskItemRegistry.set(taskItem.id, taskItem);//this.taskItems = [...this.taskItems.filter(a => a.id !== taskItem.id), taskItem];
                this.selectedTaskItem = taskItem;
                this.editMode = false;
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }


    deleteTaskItem = async (id: string) => {
        this.loading = true;

        try {
            await agent.TaskItems.delete(id)
            runInAction(() => {
                this.taskItemRegistry.delete(id);//this.taskItems = [...this.taskItems.filter(a => a.id !== id)];
                this.selectedTaskItem = undefined;
                this.editMode = false;
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }
}