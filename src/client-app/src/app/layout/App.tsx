import { useEffect, useState } from 'react';
import { Container } from 'semantic-ui-react';
import { TaskItem } from '../models/taskItem';
import NavBar from './NavBar';
import TaskItemDashboard from '../../features/taskitem/dashboard/TaskItemDashboard';
import { v4 as uuid } from 'uuid';
import agent from '../api/agent';
import LoadingComponent from './LoadingComponent';

function App() {
  const [taskItems, setTaskItems] = useState<TaskItem[]>([]);
  const [selectedTaskItems, setSelectedTaskItem] = useState<TaskItem | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    agent.TaskItems.list().then(
      response => {
        let taskItems: TaskItem[] = [];
        response.forEach(taskItem => {
          taskItem.createdAt = taskItem.createdAt.split('T')[0];
          taskItem.updatedAt = taskItem.updatedAt.split('T')[0];
          taskItems.push(taskItem);
        });
        setTaskItems(taskItems)
        setLoading(false)
      }
    );
  }, [])


  function handleSelectTaskItem(id: string) {
    setSelectedTaskItem(taskItems.find(x => x.id == id))
  }
  function handlecancelSelectedTaskItem() {
    setSelectedTaskItem(undefined)
  }
  function handleFormOpen(id?: string) {
    id ? handleSelectTaskItem(id) : handlecancelSelectedTaskItem();
    setEditMode(true);
  }
  function handlerFormClose() {
    setEditMode(false);
  }

  function handleCreateOrEditTaskItem(taskItem: TaskItem) {
  setSubmitting(true)
  if (taskItem.id) {
    agent.TaskItems.update(taskItem).then(() => {
      setTaskItems([...taskItems.filter(x => x.id !== taskItem.id), taskItem])
      setSelectedTaskItem(taskItem);
      setEditMode(false);
      setSubmitting(false)
    })
  }
  else {
    taskItem.id = uuid();
    agent.TaskItems.create(taskItem).then(() => {
      setTaskItems([...taskItems, taskItem])
      setSelectedTaskItem(taskItem);
      setEditMode(false);
      setSubmitting(false)
    })
  }
}

function handleDelete(id: string) {
    setSubmitting(true)
    agent.TaskItems.delete(id).then(() => {
      setTaskItems([...taskItems.filter(x => x.id !== id)])
      setSubmitting(false)
    })
  }

if (loading)
    return (<LoadingComponent content='Loading app...' />)
  return (
    <>
      <NavBar openForm={handleFormOpen} />
      <Container style={{ marginTop: '7em' }}>
        <TaskItemDashboard
          taskItems={taskItems}
          selectedTaskItem={selectedTaskItems}
          selectTaskItem={handleSelectTaskItem}
          cancelSelectedTaskItem={handlecancelSelectedTaskItem}
          editMode={editMode}
          openForm={handleFormOpen}
          closeForm={handlerFormClose}
          createOrEdit={handleCreateOrEditTaskItem}
          deleteTaskItem={handleDelete}
          submitting={submitting}
        />
      </Container>
    </>
  );
}

export default App;

