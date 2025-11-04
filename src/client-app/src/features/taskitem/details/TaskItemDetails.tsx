import { Button, ButtonGroup, Card, Label, Icon, Divider, SemanticCOLORS, SemanticICONS } from "semantic-ui-react";
import { TaskItem, TaskStatus } from "../../../app/models/taskItem";
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

export default observer(function TaskItemDetails() {
    const { taskItemStore } = useStore();
    const { selectedTaskItem: taskItem, loadTaskItem, loadingInitial } = taskItemStore;
    const { id } = useParams();

    useEffect(() => {
        if (id) loadTaskItem(id);
    }, [id, loadTaskItem])

    if (loadingInitial || !taskItem) return <LoadingComponent />;

    const statusColor: SemanticCOLORS = getStatusColor(taskItem.status);
    const statusIcon: SemanticICONS = getStatusIcon(taskItem.status);

    return (
        <Card fluid style={{ boxShadow: '0 4px 8px rgba(0,0,0,0.15)' }}>
            <Card.Content>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '1em' }}>
                    <Card.Header style={{ fontSize: '2em', margin: 0 }}>
                        <Icon name='tasks' style={{ marginRight: '0.5em' }} />
                        {taskItem.title}
                    </Card.Header>
                    <Label size='large' color={statusColor}>
                        <Icon name={statusIcon} />
                        {TaskStatus[taskItem.status]}
                    </Label>
                </div>
                <Divider />
                <Card.Description style={{ fontSize: '1.1em', lineHeight: '1.6', marginTop: '1em', marginBottom: '1.5em', color: '#333', minHeight: '4em' }}>
                    {taskItem.description || <em style={{ color: '#999' }}>No description provided</em>}
                </Card.Description>
                <Divider />
                <Card.Meta style={{ marginTop: '1.5em' }}>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1em' }}>
                        <div style={{ padding: '1em', backgroundColor: '#f9f9f9', borderRadius: '8px' }}>
                            <Icon name='calendar plus' color='blue' size='large' style={{ marginBottom: '0.5em' }} />
                            <div style={{ fontWeight: 'bold', color: '#666', marginBottom: '0.3em' }}>Created Date</div>
                            <div style={{ fontSize: '1.1em', color: '#333' }}>{taskItem.createdAt}</div>
                        </div>
                        <div style={{ padding: '1em', backgroundColor: '#f9f9f9', borderRadius: '8px' }}>
                            <Icon name='edit' color='orange' size='large' style={{ marginBottom: '0.5em' }} />
                            <div style={{ fontWeight: 'bold', color: '#666', marginBottom: '0.3em' }}>Last Updated</div>
                            <div style={{ fontSize: '1.1em', color: '#333' }}>{taskItem.updatedAt}</div>
                        </div>
                    </div>
                </Card.Meta>
            </Card.Content>
            <Card.Content extra style={{ backgroundColor: '#fafafa', paddingTop: '1.5em' }}>
                <ButtonGroup widths='2' fluid>
                    <Button
                        as={Link}
                        to={`/manage/${id}`}
                        color='blue'
                        icon
                        labelPosition='left'
                        size='large'
                    >
                        <Icon name='edit' />
                        Edit Task
                    </Button>
                    <Button
                        as={Link}
                        to={'/taskItems'}
                        color='grey'
                        icon
                        labelPosition='left'
                        size='large'
                    >
                        <Icon name='arrow left' />
                        Back to List
                    </Button>
                </ButtonGroup>
            </Card.Content>
        </Card>
    )
})