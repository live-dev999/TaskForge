import { observer } from "mobx-react-lite";
import { Fragment } from "react";
import { Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import TaskItemListItem from "./TaskItemListItem";

export default observer(function TaskItemList() {
    const { taskItemStore } = useStore();
    const { taskItemsByDate } = taskItemStore;

    return (
        <>
            {taskItemsByDate.length > 0 ? (
                taskItemsByDate.map(taskItem => (
                    <TaskItemListItem key={taskItem.id} taskItem={taskItem} />
                ))
            ) : (
                <Header as="h3" textAlign="center" color="grey">
                    No task items found
                </Header>
            )}
        </>
    )
})