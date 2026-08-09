using System.Security.Claims;
using InternalManagementSystem.Application.DTOs.Projects;
using InternalManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


//There is IDOR gap in prjects service and i need to fix it

namespace InternalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController: BaseApiController
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("/api/Getprojects")]
        public async Task<IActionResult> GetProjects()
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var projects = await _projectService.GetProjectsAsync(userId, role);
            return Ok(projects);
        }
        [HttpGet("/api/GetprojectByID/{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var project = await _projectService.GetProjectByIdAsync(id, userId, role);
                return Ok(project);
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

        // Only Managers and Administrators can create projects.
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost("/api/CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var project = await _projectService.CreateProjectAsync(request, userId, role);
                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager,Administrator")]
        [HttpPut("/api/UpdateProject/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var role = GetCurrentUserRole();
                var project = await _projectService.UpdateProjectAsync(id, request, userId, role);
                return Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException )
            {
                return Forbid();
            } 
        }

        [Authorize(Roles = "Manager,Administrator")]
        [HttpDelete("/api/DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                await _projectService.DeleteProjectAsync(id, userId, role);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException )
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost("/api/AddMember/{ProjectID}")]
        public async Task<IActionResult> AddMember(int id, [FromBody] AddProjectMemberDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                await _projectService.AddMemberAsync(id, request.UserId, userId, role);
                return Ok(new { message = "Member added successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException )
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "Manager,Administrator")]
        [HttpDelete("/api/RemoveMember/{id}")]
        public async Task<IActionResult> RemoveMember(int id, string userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                await _projectService.RemoveMemberAsync(id, userId, currentUserId, role);
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

    }
}
 