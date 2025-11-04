import React, { SyntheticEvent, useState } from "react";
import { Link } from "react-router-dom";
import { Card, Button, Label, Icon, ButtonGroup, SemanticCOLORS, SemanticICONS } from "semantic-ui-react";
import { TaskItem, TaskStatus } from "../../../app/models/taskItem";
import { useStore } from "../../../app/stores/store";

interface Props {
    taskItem: TaskItem
}

const getStatusColor = (status: TaskStatus): SemanticCOLORS => {
    switch (status) {
        case TaskStatus.New:
            return 'blue';
        case TaskStatus.InProgress:
            return 'orange';
        case TaskStatus.Completed:
            return 'green';
        case TaskStatus.Pending:
            return 'yellow';
        default:
            return 'grey';
    }
};

const getStatusIcon = (status: TaskStatus): SemanticICONS => {
    switch (status) {
        case TaskStatus.New:
            return 'plus circle';
        case TaskStatus.InProgress:
            return 'spinner';
        case TaskStatus.Completed:
            return 'check circle';
        case TaskStatus.Pending:
            return 'clock';
        default:
            return 'circle';
    }
};

export default function TaskItemListItem({ taskItem }: Props) {
    const [target, setTarget] = useState('');
    const { taskItemStore } = useStore();
    const { deleteTaskItem, loading } = taskItemStore;

    function handleTaskItemDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deleteTaskItem(id);
    }

    const statusColor = getStatusColor(taskItem.status);
    const statusIcon = getStatusIcon(taskItem.status);

    return (
        <Card fluid style={{ marginBottom: '1.5em', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
            <Card.Content>
                <Card.Header as={Link} to={`/taskItems/${taskItem.id}`} style={{ fontSize: '1.3em', marginBottom: '0.5em' }}>
                    <Icon name='tasks' style={{ marginRight: '0.5em' }} />
                    {taskItem.title}
                </Card.Header>
                <Card.Meta style={{ marginTop: '0.5em', marginBottom: '1em' }}>
                    <Label color={statusColor} size='small' style={{ marginRight: '0.5em' }}>
                        <Icon name={statusIcon} />
                        {TaskStatus[taskItem.status]}
                    </Label>
                </Card.Meta>
                <Card.Description style={{ minHeight: '3em', marginBottom: '1em', color: '#666' }}>
                    {taskItem.description || <em style={{ color: '#999' }}>No description provided</em>}
                </Card.Description>
                <Card.Meta style={{ marginTop: '1em', paddingTop: '1em', borderTop: '1px solid #eee' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '0.5em' }}>
                        <div style={{ display: 'flex', gap: '1.5em', flexWrap: 'wrap' }}>
                            <span style={{ color: '#888', fontSize: '0.9em' }}>
                                <Icon name='calendar plus' color='grey' />
                                <strong>Created:</strong> {taskItem.createdAt}
                            </span>
                            <span style={{ color: '#888', fontSize: '0.9em' }}>
                                <Icon name='edit' color='grey' />
                                <strong>Updated:</strong> {taskItem.updatedAt}
                            </span>
                        </div>
                    </div>
                </Card.Meta>
            </Card.Content>
            <Card.Content extra>
                <ButtonGroup fluid>
                    <Button
                        as={Link}
                        to={`/taskItems/${taskItem.id}`}
                        color='teal'
                        icon
                        labelPosition='left'
                        style={{ flex: 1 }}
                    >
                        <Icon name='eye' />
                        View Details
                    </Button>
                    <Button
                        as={Link}
                        to={`/manage/${taskItem.id}`}
                        color='blue'
                        icon
                        labelPosition='left'
                        style={{ flex: 1 }}
                    >
                        <Icon name='edit' />
                        Edit
                    </Button>
                    <Button
                        name={taskItem.id}
                        loading={loading && target === taskItem.id}
                        onClick={(e) => handleTaskItemDelete(e, taskItem.id)}
                        color='red'
                        icon
                        labelPosition='left'
                        style={{ flex: 1 }}
                    >
                        <Icon name='trash' />
                        Delete
                    </Button>
                </ButtonGroup>
            </Card.Content>
        </Card>
    )
}