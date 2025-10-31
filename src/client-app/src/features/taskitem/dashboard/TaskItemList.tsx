import { observer } from "mobx-react-lite";
import { SyntheticEvent, useState } from "react";
import { Button, Item, ItemMeta, Label, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { Link } from "react-router-dom";

export default observer(function TaskItemList() {
    const [target, setTarget] = useState('');
    const { taskItemStore } = useStore();
    const { deleteTaskItem, loading, taskItemsByDate } = taskItemStore;

    function handleTaskItemDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deleteTaskItem(id);
    }

    return (
        <Segment>
            <Item.Group divided>
                {taskItemsByDate.map((taskItem: any) => (
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
                ))}
            </Item.Group>
        </Segment>
    )
})