import React, { SyntheticEvent, useState } from "react";
import { Link } from "react-router-dom";
import { Item, ItemMeta, Button, Label } from "semantic-ui-react";
import { TaskItem } from "../../../app/models/taskItem";
import { useStore } from "../../../app/stores/store";

interface Props {
    taskItem: TaskItem
}
export default function TaskItemListItem({ taskItem }: Props) {
    const [target, setTarget] = useState('');
    const { taskItemStore } = useStore();
    const { deleteTaskItem, loading } = taskItemStore;

    function handleTaskItemDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deleteTaskItem(id);
    }

    return (
        <Item key={taskItem.id}>
            <Item.Content>
                            <Item.Header as='a'>{taskItem.title}</Item.Header>
                            <ItemMeta>
                                {taskItem.createdAt}
                            </ItemMeta>
                             <ItemMeta>
                                {taskItem.updatedAt}
                            </ItemMeta>
                            <Item.Description>
                                <div>{taskItem.description}</div>
                            </Item.Description>
                            <Item.Extra>
                                <Button as={Link} to={`/taskItems/${taskItem.id}`} floated='right' content='View' color='blue' />
                                <Button 
                                name={taskItem.id}
                                loading={loading && target === taskItem.id} 
                                onClick={(e) => handleTaskItemDelete(e, taskItem.id)} 
                                floated='right' content='Delete' color='red' />
                                <Label basic content={taskItem.status} />
                            </Item.Extra>
                        </Item.Content>
                    </Item>
    )
}