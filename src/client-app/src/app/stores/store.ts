import { useContext, createContext } from "react";
import TaskItemStore from "./taskItemStore";

interface Store {
    taskItemStore: TaskItemStore
}

export const store: Store = {
    taskItemStore: new TaskItemStore()
}

export const StoreContext = createContext(store)

export function useStore() {
    return useContext(StoreContext);
}