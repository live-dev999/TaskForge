/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.

 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios';
import { Button, Container, Header, List } from 'semantic-ui-react';
import NavBar from './app/layout/NavBar';
import TaskItemDashboard from './features/taskitem/dashboard/TaskItemDashboard';
import { TaskItem } from './app/models/taskItem';

function App() {
  const [taskItem, setTaskItems] = useState<TaskItem[]>([]);
  const [selectedTaskItems, setSelectedTaskItems] = useState<TaskItem | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);

  useEffect(() => {
    axios.get<TaskItem[]>("http://localhost:5000/api/taskItem").then(
      response => {
        setTaskItems(response.data)
      }
    );
  }, [])

  function handleSelectTaskItem(id: string) {
    setSelectedTaskItems(taskItem.find(x => x.id == id))
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
          taskItems={taskItem}
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
