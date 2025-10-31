import { makeAutoObservable } from "mobx"
import agent from "../api/agent";
import { TaskItem } from "../models/taskItem"

export default class TaskItemStore {
    taskItems: TaskItem[] = [];
    selectedTaskItem: TaskItem | null = null;
    editMode = false;
    loading = false;
    loadingInit = false;

    constructor() {
        makeAutoObservable(this)
    }

    loadTaskItems = async () => {
        this.loadingInit = true;
        try {
            const taskItems = await agent.TaskItems.list();
            taskItems.forEach(taskItem => {
                taskItem.createdAt = taskItem.createdAt.split('T')[0];
                taskItem.updatedAt = taskItem.updatedAt.split('T')[0];
                this.taskItems.push(taskItem);
            });
            this.loadingInit = false;
        } catch (error) {
            console.log(error);
            this.loadingInit = false;
        }
    }
}