using InternalManagementSystem.Application.DTOs.Tasks;
using InternalManagementSystem.Application.Interfaces;
using InternalManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternalManagementSystem.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController: BaseApiController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskservice)
        {
            _taskService = taskservice;
        }

        [HttpGet("ProjectByID/{projectid}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByProjectAsync(projectId, GetCurrentUserId(), GetCurrentUserRole());
                return Ok(tasks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpGet("MyTasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var tasks = await _taskService.GetMyTasksAsync(GetCurrentUserId());
            return Ok(tasks);
        }

        [HttpGet("TaskByID/{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id, GetCurrentUserId(), GetCurrentUserRole());
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost("CreateTask/{projectId}")]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] CreateTaskDto request)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(projectId, request, GetCurrentUserId(), GetCurrentUserRole());
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "Manager,Administrator")]
        [HttpPut("EditTask/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto request)
        {
            try
            {
                var task = await _taskService.UpdateTaskAsync(id, request, GetCurrentUserId(), GetCurrentUserRole());
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [Authorize(Roles = "Manager,Administrator")]
        [HttpDelete("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id, GetCurrentUserId(), GetCurrentUserRole());
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpPut("ChangeTaskStatus{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeTaskStatusDto request)
        {
            try
            {
                var task = await _taskService.ChangeStatusAsync(id, request, GetCurrentUserId(), GetCurrentUserRole());
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

    }
}
