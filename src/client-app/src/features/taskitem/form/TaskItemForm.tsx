import React from "react";
import { Button, Form, Segment } from "semantic-ui-react";

export default function TaskItemForm(){
    return(
        <Segment clearing>
            <Form>
                <Form.Input placeholder='Title'/>
                <Form.TextArea placeholder='Description' />
                <Form.Input placeholder='Create Date' />
                <Form.Input placeholder='Update Date' />
                <Form.Input placeholder='Status' />
                <Button floated='right' positive type='submit' content='Submit'/>
                <Button floated='right' positive type='button' content='Cancel'/>
            </Form>
        </Segment>
    )
}