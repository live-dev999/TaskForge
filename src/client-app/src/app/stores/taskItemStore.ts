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

    loadTaskItem = async (id: string) => {
        let taskItem = this.getTaskItem(id)
        if (taskItem) this.selectedTaskItem = taskItem;
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
                console.log(error);
                this.setLoadingInitial(false);
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
            await agent.TaskItems.create(taskItem)
            runInAction(() => {
                this.taskItemRegistry.set(taskItem.id, taskItem);//this.taskItems.push(taskItem);
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