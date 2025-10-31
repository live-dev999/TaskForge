import { ChangeEvent, useEffect, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { TaskItem, TaskStatus } from "../../../app/models/taskItem";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function TaskItemForm() {
    const { taskItemStore } = useStore();
    const { createTaskItem, updateTaskItem, loading, loadTaskItem, loadingInitial } = taskItemStore;
    const { id } = useParams();
    const [taskItem, setTaskItem] = useState<TaskItem>({
        id: '',
        title: '',
        description: '',
        createdAt: '',
        updatedAt:'',
        status: TaskStatus.New
});
const navigate = useNavigate();
    useEffect(() => {
        if (id) loadTaskItem(id).then(taskItem => setTaskItem(taskItem!));
    }, [id, loadTaskItem])


    function handleSubmit() {

        if (!taskItem.id) {
            taskItem.id = uuid();
            createTaskItem(taskItem).then(
                () => {
                    navigate(`/taskItems/${taskItem.id}`)
                }
            );
        }
        else {
            updateTaskItem(taskItem).then(
                () => {
                    navigate(`/taskItems/${taskItem.id}`)
                }
            );
        }
    }

    function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
        const { name, value } = event.target;
        setTaskItem({ ...taskItem, [name]: value });
    }
    if (loadingInitial) return <LoadingComponent content="Loading taskItem ..." />
    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit}>
                <Form.Input placeholder='Title' value={taskItem.title} name='title' onChange={handleInputChange} />
                <Form.TextArea placeholder='Description' value={taskItem.description} name='description' onChange={handleInputChange} />
                <Form.Input type="date" placeholder='Create Date' value={taskItem.createdAt} name='createdAt' onChange={handleInputChange} />
                <Form.Input type="date" placeholder='Update Date' value={taskItem.updatedAt} name='updatedAt' onChange={handleInputChange} />
                <Form.Input placeholder='Status' value={taskItem.status} name='status' onChange={handleInputChange} />
                 <Button loading={loading} floated='right' positive type='submit' content='Submit' />
                <Button floated='right' as={Link} to={'/taskItems'}  positive type='button' content='Cancel' />
            </Form>
        </Segment>
    )
})

function uuid(): string {
    throw new Error("Function not implemented.");
}
