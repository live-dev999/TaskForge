import { makeAutoObservable } from "mobx"

export default class TaskItemStore {
    title = 'hello'

    constructor() {
        makeAutoObservable(this)
    }

    setTitle = () => {
        this.title = this.title + "!";
    }
}