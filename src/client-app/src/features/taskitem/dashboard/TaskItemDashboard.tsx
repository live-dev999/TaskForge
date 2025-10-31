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

import React from "react";
import { Grid, List } from "semantic-ui-react";
import TaskItemList from "./TaskItemList";
import { TaskItem } from "../../../app/models/taskItem";
import TaskItemDetails from "../details/TaskItemDetails";

interface Props{
    taskItems: TaskItem[]
}
export default function TaskItemDashboard({taskItems}: Props) {
    return (
        <Grid>
            <Grid.Column width='10'>
                <TaskItemList taskItems={taskItems}/>
            </Grid.Column>
            <Grid.Column width='6'>
                {taskItems[0] &&
                <TaskItemDetails taskItem={taskItems[0]}/>}
            </Grid.Column>
        </Grid>
        )
}