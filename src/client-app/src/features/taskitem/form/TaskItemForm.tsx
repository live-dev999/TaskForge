import { ChangeEvent, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { TaskItem } from "../../../app/models/taskItem";


interface Props {
    taskItem: TaskItem | undefined;
    closeForm: () => void;
}


export default function TaskItemyForm({ taskItem: selectedTaskItemy, closeForm }: Props) {
    
    const initialState = selectedTaskItemy ?? {
        id: '',
        title: '',
        description: '',
        createdAt: '',
        updatedAt:'',
        status: ''
    }

    const [taskItem,setTaskItem] = useState(initialState);

    function handleSubmit(){
        console.log(taskItem)
    }

    function handleInputChange(event:ChangeEvent<HTMLInputElement | HTMLTextAreaElement>){
        const{name,value} = event.target;
        setTaskItem({...taskItem,[name]:value});
    }

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit}>
                <Form.Input placeholder='Title' value={taskItem.title} onChange={handleInputChange}/>
                <Form.TextArea placeholder='Description' value={taskItem.description} name='description' onChange={handleInputChange} />
                <Form.Input type="date" placeholder='Create Date' value={taskItem.createdAt} name='createDate' onChange={handleInputChange} />
                <Form.Input type="date" placeholder='Update Date' value={taskItem.updatedAt} name='updateDate' onChange={handleInputChange} />
                <Form.Input placeholder='Status' value={taskItem.status} name='status' onChange={handleInputChange} />
                <Button floated='right' positive type='submit' content='Submit' />
                <Button onClick={closeForm} floated='right' positive type='button' content='Cancel' />
            </Form>
        </Segment>
    )
}