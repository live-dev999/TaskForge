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
import { Button, Item, ItemMeta, Label, Segment } from "semantic-ui-react";
import { TaskItem } from "../../../app/models/taskItem";


interface Props {
    taskItems: TaskItem[];
    selectTaskItem: (id: string) => void;
}

export default function TaskItemList({ taskItems, selectTaskItem }: Props) {
    return (
        <Segment>
            <Item.Group divided>
                {taskItems.map((taskItem) => (
                    <Item key={taskItem.id}>
                        <Item.Content>
                            <Item.Header as='a'>{taskItem.title}</Item.Header>
                            <ItemMeta>
                                {taskItem.createdAt}
                            </ItemMeta>
                            <ItemMeta>
                                {taskItem.updatedAt}
                            </ItemMeta>
                            <Item.Description>
                                <div>{taskItem.description}</div>
                            </Item.Description>
                            <Item.Extra>
                                <Button onClick={() => selectTaskItem(taskItem.id)} floated='right' content='View' color='blue' />
                                <Label basic content={taskItem.status}/>
                            </Item.Extra>
                        </Item.Content>
                    </Item>
               ))}
            </Item.Group>
        </Segment>
    )
}