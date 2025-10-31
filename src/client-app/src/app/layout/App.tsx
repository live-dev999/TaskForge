import { useEffect, useState } from 'react';
import { Container } from 'semantic-ui-react';
import { TaskItem } from '../models/taskItem';
import NavBar from './NavBar';
import TaskItemDashboard from '../../features/taskitem/dashboard/TaskItemDashboard';
import { v4 as uuid } from 'uuid';
import agent from '../api/agent';
import LoadingComponent from './LoadingComponent';
import { observer } from 'mobx-react-lite';
import { useStore } from '../stores/store';

function App() {

  const {taskItemStore} = useStore();

  const [taskItems, setTaskItems] = useState<TaskItem[]>([]);
  const [selectedTaskItems, setSelectedTaskItem] = useState<TaskItem | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  useEffect(() => {
    taskItemStore.loadTaskItems();
  }, [taskItemStore])

  function handleSelectTaskItem(id: string) {
    setSelectedTaskItem(taskItems.find(x => x.id === id))
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

    // taskItem.id ? setTaskItems([...taskItems.filter(x => x.id !== taskItem.id), taskItem])
    //   : setTaskItems([...taskItems, { ...taskItem, id: uuid() }])
    // setEditMode(false);
    // setSelectedTaskItem(taskItem);
  }

  function handleDelete(id: string) {
    setSubmitting(true)
    agent.TaskItems.delete(id).then(() => {
      setTaskItems([...taskItems.filter(x => x.id !== id)])
      setSubmitting(false)
    })
  }

  if (taskItemStore.loadingInitial)
    return (<LoadingComponent content='Loading app...' />)
  return (
    <>
      <NavBar openForm={handleFormOpen} />
      
      <Container style={{ marginTop: '7em' }}>
   
        <TaskItemDashboard
          taskItems={taskItemStore.taskItems}
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

export default observer(App);
