import { observer } from "mobx-react-lite";
import { Fragment, SyntheticEvent, useState } from "react";
import { Button, Header, Item, ItemMeta, Label, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { Link } from "react-router-dom";
import TaskItemListItem from "./TaskItemListItem";

export default observer(function TaskItemList() {
    const { taskItemStore } = useStore();
    const { groupedTaskItems } = taskItemStore;


    return (
        <>
            {groupedTaskItems.map(([group, taskItems]) => (
                <Fragment key={group}>
                    <Header sub color="teal">
                        {group}
                    </Header>


                    {taskItems.map(taskItem => (
                        <TaskItemListItem key={taskItem.id} taskItem={taskItem} />
                    ))}
                </Fragment>
            ))}
        </>

    )
})