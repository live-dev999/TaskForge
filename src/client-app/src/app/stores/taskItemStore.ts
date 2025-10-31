import { makeAutoObservable } from "mobx"
import agent from "../api/agent";
import { TaskItem } from "../models/taskItem"

export default class TaskItemStore {
    taskItems: TaskItem[] = [];
    selectedTaskItem: TaskItem | null = null;
    editMode = false;
    loading = false;
    loadingInitial = false;

    constructor() {
        makeAutoObservable(this)
    }

    loadTaskItems = async () => {
        this.setLoadingInitial(true);
        try {
            const taskItems = await agent.TaskItems.list();

            taskItems.forEach(taskItem => {
                taskItem.createdAt = taskItem.createdAt.split('T')[0];
                taskItem.updatedAt = taskItem.updatedAt.split('T')[0];
                this.taskItems.push(taskItem);
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
}