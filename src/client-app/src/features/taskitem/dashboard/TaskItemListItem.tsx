import React, { SyntheticEvent, useState } from "react";
import { Link } from "react-router-dom";
import { Item, ItemMeta, Button, Label, SegmentGroup, Segment, Icon } from "semantic-ui-react";
import { TaskItem, TaskStatus } from "../../../app/models/taskItem";
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
         <SegmentGroup>
            <Segment>
                 <Item.Group>
                    <Item.Content>
                        <Item.Header as={Link} to={`/taskItems/${taskItem.id}`}>
                            {taskItem.title}
                        </Item.Header>
                        <Item.Description> 
                            {taskItem.description}
                        </Item.Description>
                    </Item.Content>
                 </Item.Group>
                  </Segment>
                  <Segment>
                <span>
                    Create Date:
                    <Icon name="clock" /> {taskItem.updatedAt}
                    <br></br>
                     Update Date:
                    <Icon name="clock" /> {taskItem.updatedAt}
                </span>
               
            </Segment>
             <Segment secondary>
                Task Status: {TaskStatus[taskItem.status]}
            </Segment>   
            <Segment clearing>
          
                <Button as={Link} to={`/taskItems/${taskItem.id}`}
                    color='teal'
                    floated='right'
                    content='View' />
                <Button
                    name={taskItem.id}
                    loading={loading && target === taskItem.id}
                    onClick={(e) => handleTaskItemDelete(e, taskItem.id)}
                    floated='right' content='Delete' color='red' />
                <Label basic content={taskItem.status} />
            </Segment>
        {/* <Item key={taskItem.id}>
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
                    </Item> */}
              
         </SegmentGroup>
    )
}