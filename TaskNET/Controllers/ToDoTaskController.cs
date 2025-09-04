using Microsoft.AspNetCore.Mvc;
using TaskNET.Interfaces;
using TaskNET.Models;

namespace TaskNET.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class ToDoTaskController(IAppDataProvider appDataProvider) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetToDoTasks()
        {
            var result = await appDataProvider.GetToDoTasksAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDoTask(int id)
        {
            var result = await appDataProvider.GetToDoTaskAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncomingToDoTasks(IncomingTasksFilter incomingTasksFilter = IncomingTasksFilter.Today)
        {
            var result = await appDataProvider.GetIncomingToDoTasksAsync(incomingTasksFilter);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToDoTask([FromBody] ToDoTask todoTask)
        {
            var result = await appDataProvider.CreateToDoTaskAsync(todoTask);
            return CreatedAtAction("GetToDoTask", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDoTask(int id, [FromBody] ToDoTask toDoTask)
        {
            if (id != toDoTask.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var updatedTask = await appDataProvider.UpdateToDoTaskAsync(toDoTask);
            if (updatedTask == null)
            {
                return NotFound();
            }
            return Ok(updatedTask);
        }

        [HttpPatch("{id}/percent")]
        public async Task<IActionResult> SetToDoTaskProgress(int id, decimal percent)
        {
            var result = await appDataProvider.SetToDoTaskProgressAsync(id, percent);
            if (result == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPatch("{id}/done")]
        public async Task<IActionResult> MarkToDoTaskAsDone(int id)
        {
            var result = await appDataProvider.MarkToDoTaskAsDoneAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoTask(int id)
        {
            var success = await appDataProvider.DeleteToDoTaskAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}