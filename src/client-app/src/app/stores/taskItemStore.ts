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
        try {
            const taskItems = await agent.TaskItems.list();

            taskItems.forEach(taskItem => {
                taskItem.createdAt = taskItem.createdAt.split('T')[0];
                taskItem.updatedAt = taskItem.updatedAt.split('T')[0];
                this.taskItemRegistry.set(taskItem.id, taskItem);
            });
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    selectTaskItem = (id: string) => {
        this.selectedTaskItem = this.taskItemRegistry.get(id);//this.taskItems.find(a => a.id === id);

    }
    cancelselectedTaskItem = () => {
        this.selectedTaskItem = undefined;
    }

    openForm = (id?: string) => {
        id ? this.selectTaskItem(id) : this.cancelselectedTaskItem();
        this.editMode = true;
    }
    closeForm = () => {
        this.editMode = false;
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