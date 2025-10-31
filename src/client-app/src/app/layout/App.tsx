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

  useEffect(() => {
    agent.TaskItems.list().then(
      response => {
        let activities: TaskItem[] = [];
        response.forEach(taskItem => {
          taskItem.createdAt = taskItem.createdAt.split('T')[0];
          taskItem.updatedAt = taskItem.updatedAt.split('T')[0];
          activities.push(taskItem);
        });
        setTaskItems(activities)
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
    taskItem.id ? setTaskItems([...taskItems.filter(x => x.id !== taskItem.id), taskItem])
      : setTaskItems([...taskItems, { ...taskItem, id: uuid() }])
    setEditMode(false);
    setSelectedTaskItem(taskItem);
  }

  function handleDelete(id: string) {
    setTaskItems([...taskItems.filter(x => x.id !== id)])
  }

  if(loading)
  return (<LoadingComponent content='Loading app...'/>)
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
          deleteTaskItem={handleDelete}
        />
      </Container>
    </>
  );
}

export default App;
