using Application.TaskItems;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Domain;


namespace TaskForge.API.Controllers
{
    public class TaskItemController : BaseApiController
    {
        [HttpGet] //api/taskitems
        public async Task<ActionResult<List<TaskItem>>> GetTaskItems()
        {
            return await Mediator.Send(new List.Query());
        }

        [HttpGet("{id}")] //api/taskitems/{id}
        public async Task<ActionResult<TaskItem>> GetTaskItem(Guid id)
        {
            return await Mediator.Send(new Details.Query() { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskItem([FromBody] TaskItem TaskItem)
        {
            return Ok(await Mediator.Send(new Create.Command { TaskItem = TaskItem }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTaskItem(Guid id, [FromBody] TaskItem TaskItem)
        {
            TaskItem.Id = id;
            return Ok(await Mediator.Send(new Edit.Command { TaskItem = TaskItem }));
        }
    }
}
