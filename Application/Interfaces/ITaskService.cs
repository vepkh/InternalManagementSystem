using InternalManagementSystem.Application.DTOs.Tasks;

namespace InternalManagementSystem.Application.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskResponseDto>> GetTasksByProjectAsync(int projectId, string currentUserId, string currentUserRole);
        Task<List<TaskResponseDto>> GetMyTasksAsync(string currentUserId);
        Task<TaskResponseDto> GetTaskByIdAsync(int taskId, string currentUserId, string currentUserRole);
        Task<TaskResponseDto> CreateTaskAsync(int projectId, CreateTaskDto request, string currentUserId, string currentUserRole);
        Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskDto request, string currentUserId, string currentUserRole);
        Task DeleteTaskAsync(int taskId, string currentUserId, string currentUserRole);
        Task<TaskResponseDto> ChangeStatusAsync(int taskId, ChangeTaskStatusDto request, string currentUserId, string currentUserRole);
    }
}
