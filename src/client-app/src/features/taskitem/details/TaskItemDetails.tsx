import { Button, ButtonGroup, Card } from "semantic-ui-react";
import { TaskItem } from "../../../app/models/taskItem";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { Link, useParams } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";

interface Props {
    taskItem: TaskItem;
    cancelSelectedTaskItem: () => void;
    openForm: (id: string) => void;
}


export default observer(function TaskItemDetails() {
    const { taskItemStore } = useStore();
    const { selectedTaskItem: taskItem, loadTaskItem, loadingInitial } = taskItemStore;
    const { id } = useParams();

    useEffect(() => {
        if (id) loadTaskItem(id);
    }, [id, loadTaskItem])

    if (loadingInitial || !taskItem) return <LoadingComponent />;

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
                    <Button as={Link} to={`/manage/${id}`} basic color='blue' content='Edit' />
                    <Button as={Link} to={'/taskItems'} basic color='green' content='Cancel' />
                </ButtonGroup>
            </Card.Content>
        </Card >
    )
})