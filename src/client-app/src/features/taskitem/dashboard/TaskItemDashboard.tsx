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

import { Grid } from "semantic-ui-react";
import TaskItemList from "./TaskItemList";
import { TaskItem } from "../../../app/models/taskItem";
import TaskItemDetails from "../details/TaskItemDetails";
import TaskItemForm from "../form/TaskItemForm";

interface Props {
    taskItems: TaskItem[];
    selectedTaskItem: TaskItem | undefined;
    selectTaskItem: (id: string) => void;
    cancelSelectedTaskItem: () => void;
    editMode: boolean;
    openForm: (id: string) => void;
    closeForm: () => void;
}

export default function TaskItemDashboard({ taskItems, selectedTaskItem,
    selectTaskItem, cancelSelectedTaskItem, editMode, openForm, closeForm }: Props) {
    return (
        <Grid>
            <Grid.Column width='10'>
                <TaskItemList taskItems={taskItems} selectTaskItem={selectTaskItem} />
            </Grid.Column>
            <Grid.Column width='6'>
                {selectedTaskItem && !editMode &&
                    <TaskItemDetails
                        taskItem={selectedTaskItem}
                        cancelSelectedTaskItem={cancelSelectedTaskItem}
                        openForm={openForm} />}
                {editMode &&
                 <TaskItemForm
                    closeForm={closeForm}
                    taskItem={selectedTaskItem} />}
            </Grid.Column>

        </Grid>
    )
}