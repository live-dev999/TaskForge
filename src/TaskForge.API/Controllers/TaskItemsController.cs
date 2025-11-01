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

using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.TaskItems;
using TaskForge.Domain;

namespace TaskForge.API.Controllers;

public class TaskItemsController : BaseApiController
{
    [HttpGet] //api/taskitems
    public async Task<IActionResult> GetTaskItems(CancellationToken ct)
    {
        return HandleResult(await Mediator.Send(new List.Query(), ct));
    }

    [HttpGet("{id}")] //api/taskitems/{id}
    public async Task<IActionResult> GetTaskItem(Guid id)
    {
        return HandleResult(await Mediator.Send(new Details.Query() { Id = id }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskItem([FromBody] TaskItem TaskItem)
    {
        return HandleResult(await Mediator.Send(new Create.Command { TaskItem = TaskItem }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditTaskItem(Guid id, [FromBody] TaskItem TaskItem)
    {
        TaskItem.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command { TaskItem = TaskItem }));
    }

    [HttpDelete("{id}")] //api/taskitems/{id}
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }
}
