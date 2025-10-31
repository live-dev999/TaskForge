import { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { Button, Container, Header, List } from 'semantic-ui-react';
import { TaskItem } from '../models/taskItem';
import NavBar from './NavBar';
import TaskItemDashboard from '../../features/taskitem/dashboard/TaskItemDashboard';


function App() {
  const [taskItems, setTaskItems] = useState<TaskItem[]>([]);
  const [selectedTaskItems, setSelectedTaskItems] = useState<TaskItem | undefined>(undefined);

  useEffect(() => {
    axios.get<TaskItem[]>("http://localhost:5000/api/taskItems").then(
      response => {
        setTaskItems(response.data)
      }
    );
  }, [])

  function handleSelectedTaskItem(id: string){
    setSelectedTaskItems(taskItems.find(x=>x.id == id ))
  }
  function handlecancelSelectedTaskItem() {
    setSelectedTaskItems(undefined)
  }
  return (
    <>
      <NavBar />
      <Container style={{ marginTop: '7em' }}>
        <TaskItemDashboard 
        taskItem={taskItems}
          selectedTaskItem={selectedTaskItems}
          selectTaskItem={handleSelectedTaskItem}
          cancelSelectedTaskItem={handlecancelSelectedTaskItem}
        />
      </Container>
    </>
  );
}

export default App;
