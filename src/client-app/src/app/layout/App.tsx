import { useEffect } from 'react';
import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import LoadingComponent from './LoadingComponent';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';
import TaskItemDashboard from '../../features/taskitem/dashboard/TaskItemDashboard';

function App() {

  const {taskItemStore} = useStore();

  useEffect(() => {
    taskItemStore.loadTaskItems();
  }, [taskItemStore])

  if (taskItemStore.loadingInitial)
    return (<LoadingComponent content='Loading app...' />)
  return (
    <>
      <NavBar/>
      
      <Container style={{ marginTop: '7em' }}>
   
        <TaskItemDashboard
        />
      </Container>
    </>
  );
}

export default observer(App);