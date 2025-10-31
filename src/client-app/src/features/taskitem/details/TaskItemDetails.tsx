import { Button, ButtonGroup, Card } from "semantic-ui-react";
import { TaskItem } from "../../../app/models/taskItem";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";

interface Props {
    taskItem: TaskItem;
    cancelSelectedTaskItem: () => void;
    openForm: (id: string) => void;
}


export default function TaskItemDetails() {
    const { taskItemStore } = useStore();
    const { selectedTaskItem: taskItem} = taskItemStore;

    if (!taskItem) return <LoadingComponent/>;

    return (
        <Card fluid>
            <Card.Content>
                <Card.Header>{taskItem.title}</Card.Header>
                <Card.Meta>
                    <span>{taskItem.createdAt}</span>
                </Card.Meta>
               <Card.Meta>
                    <span>{taskItem.updatedAt}</span>
                </Card.Meta>
                <Card.Description>
                    {taskItem.description}
                </Card.Description>
            </Card.Content>
             <Card.Content extra>
                <ButtonGroup widths='2'>
                    <Button basic color='blue' content='Edit' />
                    <Button basic color='green' content='Cancel' />
                </ButtonGroup>
        </Card.Content>
        </Card >
    )
}