import { ChangeEvent, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { TaskItem, TaskStatus } from "../../../app/models/taskItem";


interface Props {
    taskItem: TaskItem | undefined;
    closeForm: () => void;
    createOrEdit: (taskItem: TaskItem) => void;
    submitting: boolean;
}

export default function TaskItemForm({ taskItem: selectedTaskItem, closeForm, createOrEdit, submitting }: Props) {
    
    const initialState = selectedTaskItem ?? {
        id: '',
        title: '',
        description: '',
        createdAt: '',
        updatedAt:'',
        status: TaskStatus.New
    }

    const [taskItem, setTaskItem] = useState(initialState);

    function handleSubmit() {
        console.log(taskItem)
        createOrEdit(taskItem)
    }

    function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
        const { name, value } = event.target;
        setTaskItem({ ...taskItem, [name]: value });
    }

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit}>
                <Form.Input placeholder='Title' value={taskItem.title} name='title' onChange={handleInputChange} />
                <Form.TextArea placeholder='Description' value={taskItem.description} name='description' onChange={handleInputChange} />
                <Form.Input type="date" placeholder='Create Date' value={taskItem.createdAt} name='createdAt' onChange={handleInputChange} />
                <Form.Input type="date" placeholder='Update Date' value={taskItem.updatedAt} name='updatedAt' onChange={handleInputChange} />
                <Form.Input placeholder='Status' value={taskItem.status} name='status' onChange={handleInputChange} />
               <Button loading={submitting} floated='right' positive type='submit' content='Submit' />
                <Button onClick={closeForm} floated='right' positive type='button' content='Cancel' />
            </Form>
        </Segment>
    )
}