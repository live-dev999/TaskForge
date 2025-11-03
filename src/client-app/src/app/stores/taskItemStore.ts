import { makeAutoObservable, runInAction } from "mobx"
import agent from "../api/agent";
import { TaskItem } from "../models/taskItem"
import { v4 as uuid } from 'uuid';

export default class TaskItemStore {
    taskItemRegistry = new Map<string, TaskItem>();
    selectedTaskItem: TaskItem | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = true;

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


    loadTaskItems = async () => {
        this.setLoadingInitial(true);
        try {
            const taskItems = await agent.TaskItems.list();

            taskItems.forEach(taskItem => {
                this.setTaskItem(taskItem);
            });
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
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

    createTaskItem = async (taskItem: TaskItem) => {
        this.loading = true;
        taskItem.id = uuid();

        try {
            const createdTaskItem = await agent.TaskItems.create(taskItem)
            runInAction(() => {
                // Use the created task item from API (with correct CreatedAt/UpdatedAt from server)
                const finalTaskItem = createdTaskItem || taskItem;
                this.taskItemRegistry.set(finalTaskItem.id, finalTaskItem);
                this.selectedTaskItem = finalTaskItem;
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