import { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { Button, Container, Header, List } from 'semantic-ui-react';
import { TaskItem } from '../models/taskItem';
import NavBar from './NavBar';
import TaskItemDashboard from '../../features/taskitem/dashboard/TaskItemDashboard';


function App() {
  const [taskItems, setTaskItems] = useState<TaskItem[]>([]);
  const [selectedTaskItems, setSelectedTaskItems] = useState<TaskItem | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);

  useEffect(() => {
    axios.get<TaskItem[]>("http://localhost:5000/api/taskItems").then(
      response => {
        setTaskItems(response.data)
      }
    );
  }, [])

  function handleSelectTaskItem(id: string) {
    setSelectedTaskItems(taskItems.find(x => x.id == id))
  }
  function handlecancelSelectedTaskItem() {
    setSelectedTaskItems(undefined)
  }
  function handleFormOpen(id?: string) {
    id ? handleSelectTaskItem(id) : handlecancelSelectedTaskItem();
    setEditMode(true);
  }
  function handlerFormClose() {
    setEditMode(false);
  }

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
          
        />
      </Container>
    </>
  );
}

export default App;
