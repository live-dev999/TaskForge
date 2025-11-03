import { useContext, createContext } from "react";
import TaskItemStore from "./taskItemStore";
import CommonStore from "./commonStore";

interface Store {
    taskItemStore: TaskItemStore;
    commonStore: CommonStore;
}

export const store: Store = {
    taskItemStore: new TaskItemStore(),
    commonStore: new CommonStore()
}

export const StoreContext = createContext(store)

export function useStore() {
    return useContext(StoreContext);
}