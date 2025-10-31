import { observer } from "mobx-react-lite";
import { SyntheticEvent, useState } from "react";
import { Button, Item, ItemMeta, Label, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { Link } from "react-router-dom";
import TaskItemListItem from "./TaskItemListItem";

export default observer(function TaskItemList() {
    const { taskItemStore } = useStore();
    const { taskItemsByDate } = taskItemStore;


    return (
        <Segment>
            <Item.Group divided>
                {taskItemsByDate.map(taskItem => (
                    <TaskItemListItem key={taskItem.id} taskItem={taskItem} />
                ))}
            </Item.Group>
        </Segment>
    )
})