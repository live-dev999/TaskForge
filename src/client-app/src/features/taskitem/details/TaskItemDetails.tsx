import React from "react";
import { Button, ButtonGroup, Card, Icon, Image } from "semantic-ui-react";
import { TaskItem } from "../../../app/models/taskItem";

interface Props {
    taskItem: TaskItem
}

export default function TaskItemDetails({ taskItem }: Props) {
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
                    <Button basic color='blue' content='Edit'></Button>
                    <Button basic color='green' content='Cancel'></Button>
            </ButtonGroup>
        </Card.Content>
        </Card >
    )
}