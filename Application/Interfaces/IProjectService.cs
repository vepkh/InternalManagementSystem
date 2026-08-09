using InternalManagementSystem.Application.DTOs.Projects;

namespace InternalManagementSystem.Application.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectResponseDto>> GetProjectsAsync(string currentUserId, string currentUserRole);
        Task<ProjectResponseDto> GetProjectByIdAsync(int projectId, string currentUserId, string currentUserRole);
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto request, string currentUserId, string currentUserRole);
        Task<ProjectResponseDto> UpdateProjectAsync(int projectId, UpdateProjectDto request, string currentUserId, string currentUserRole);
        Task DeleteProjectAsync(int projectId, string currentUserId, string currentUserRole);
        Task AddMemberAsync(int projectId, string userId, string currentUserId, string currentUserRole);
        Task RemoveMemberAsync(int projectId, string userId, string currentUserId, string currentUserRole);
    }
}
