using InternalManagementSystem.Application.DTOs.Tasks;
using InternalManagementSystem.Application.Interfaces;
using InternalManagementSystem.Domain.Models;
using InternalManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InternalManagementSystem.Application.Services
{
    public class TaskService:ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskResponseDto>> GetTasksByProjectAsync(int projectId, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found");

            EnsureCanViewProject(project, currentUserId, currentUserRole);


            var tasks = await _context.Tasks
           .Include(t => t.Project)
           .Include(t => t.AssignedToUser)
           .Include(t => t.CreatedByUser)
           .Where(t => t.ProjectId == projectId)
           .ToListAsync();

            return tasks.Select(MapToDto).ToList();



        }
        public async Task<List<TaskResponseDto>> GetMyTasksAsync(string currentUserId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Where(t => t.AssignedToUserId == currentUserId)
                .ToListAsync();

            return tasks.Select(MapToDto).ToList();
        }
        public async Task<TaskResponseDto> GetTaskByIdAsync(int taskId, string currentUserId, string currentUserRole)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.ProjectMembers)
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new KeyNotFoundException("Task not found.");

            EnsureCanViewProject(task.Project, currentUserId, currentUserRole);

            return MapToDto(task);
        }
        public async Task<TaskResponseDto> CreateTaskAsync(int projectId, CreateTaskDto request, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects.FindAsync(projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            EnsureCanManageProject(project, currentUserId, currentUserRole);

            if (!string.IsNullOrWhiteSpace(request.AssignedToUserId))
            {
                var assigneeExists = await _context.Users.AnyAsync(u => u.Id == request.AssignedToUserId);
                if (!assigneeExists)
                    throw new InvalidOperationException("Assigned user does not exist.");
            }

            var task = new TaskItem
            {
                ProjectId = projectId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                DueDate = request.DueDate,
                AssignedToUserId = request.AssignedToUserId,
                CreatedByUserId = currentUserId,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id, currentUserId, currentUserRole);
        }
        public async Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskDto request, string currentUserId, string currentUserRole)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new KeyNotFoundException("Task not found.");

            EnsureCanManageProject(task.Project, currentUserId, currentUserRole);

            if (!string.IsNullOrWhiteSpace(request.AssignedToUserId))
            {
                var assigneeExists = await _context.Users.AnyAsync(u => u.Id == request.AssignedToUserId);
                if (!assigneeExists)
                    throw new InvalidOperationException("Assigned user does not exist.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.DueDate = request.DueDate;
            task.AssignedToUserId = request.AssignedToUserId;
            task.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id, currentUserId, currentUserRole);
        }
        public async Task DeleteTaskAsync(int taskId, string currentUserId, string currentUserRole)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new KeyNotFoundException("Task not found.");

            EnsureCanManageProject(task.Project, currentUserId, currentUserRole);

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        public async Task<TaskResponseDto> ChangeStatusAsync(int taskId, ChangeTaskStatusDto request, string currentUserId, string currentUserRole)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new KeyNotFoundException("Task not found.");

            // This is the key rule from the spec: Administrator/the project's Manager
            // can change status on ANY task in their project. An Employee can ONLY
            // change status on a task specifically assigned to them.
            bool isManagerOrAdmin = currentUserRole == "Administrator" || task.Project.ManagerId == currentUserId;
            bool isAssignee = task.AssignedToUserId == currentUserId;

            if (!isManagerOrAdmin && !isAssignee)
                throw new UnauthorizedAccessException("You can only change the status of tasks assigned to you.");

            var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
            if (!validStatuses.Contains(request.Status))
                throw new InvalidOperationException($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");

            task.Status = request.Status;
            task.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id, currentUserId, currentUserRole);
        }


        //helpers
        private static void EnsureCanViewProject(Project project, string currentUserId, string currentUserRole)
        {
            if (currentUserRole == "Administrator") { return; }

            bool isManager = project.ManagerId == currentUserRole;
            bool isMember = project.ProjectMembers.Any(pm => pm.UserId == currentUserId);

            if (!isManager && !isMember)
                throw new UnauthorizedAccessException("You do not have access to this project's tasks.");

        }
        private static void EnsureCanManageProject(Project project, string currentUserId, string currentUserRole)
        {
            if (currentUserRole == "Administrator")
                return;

            if (project.ManagerId != currentUserId)
                throw new UnauthorizedAccessException("You are not the manager of this project.");
        }
        private static TaskResponseDto MapToDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser?.FullName,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUser.FullName,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate,
                UpdatedDate = task.UpdatedDate
            };
        }
    }
}
