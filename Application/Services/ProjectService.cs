using InternalManagementSystem.Application.DTOs.Projects;
using InternalManagementSystem.Application.Interfaces;
using InternalManagementSystem.Domain.Models;
using InternalManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;



namespace InternalManagementSystem.Application.Services



{
    public class ProjectService:IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProjectResponseDto>> GetProjectsAsync(string currentUserId, string currentUserRole)
        {
            var query = _context.Projects
            .Include(p => p.Manager)
            .Include(p => p.ProjectStatus)
            .Include(p => p.ProjectMembers)
                .ThenInclude(pm => pm.User)
            .AsQueryable();

            if(currentUserRole == "Manager")//Manager only sees his projects
            {
                query = query.Where(p => p.ManagerId == currentUserRole);
            }
            else if (currentUserRole == "Employee")
            {
                query = query.Where(p => p.ProjectMembers.Any(pm => pm.UserId == currentUserId));
            }

            var projects = await query.ToListAsync();

            return projects.Select(MapToDto).ToList();
        }
        public async Task<ProjectResponseDto> GetProjectByIdAsync(int projectId, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.ProjectStatus)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            EnsureCanView(project, currentUserId, currentUserRole);

            return MapToDto(project);
        }

        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto request, string currentUserId, string currentUserRole)
        {
            string managerId;

            if (currentUserRole == "Manager")
            {
                // Managers always become the manager of projects they create.
                managerId = currentUserId;
            }
            else
            {
                // Administrator must specify who the manager is.
                if (string.IsNullOrWhiteSpace(request.ManagerId))
                    throw new InvalidOperationException("ManagerId is required when an Administrator creates a project.");

                var managerUser = await _context.Users.FindAsync(request.ManagerId)
                    ?? throw new InvalidOperationException("Specified manager does not exist.");

                var managerRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == request.ManagerId)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToListAsync();

                if (!managerRoles.Contains("Manager"))
                    throw new InvalidOperationException("Specified user is not a Manager.");

                managerId = request.ManagerId;
            }

            var statusExists = await _context.Projectstatuses.AnyAsync(s => s.ProjectStatusId == request.ProjectStatusId);
            if (!statusExists)
                throw new InvalidOperationException("Invalid ProjectStatusId.");

            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ProjectStatusId = request.ProjectStatusId,
                ManagerId = managerId,
                CreatedDate = DateTime.Now
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(project.Id, currentUserId, currentUserRole);
        }
        public async Task<ProjectResponseDto> UpdateProjectAsync(int projectId, UpdateProjectDto request, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects.FindAsync(projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            EnsureCanManage(project, currentUserId, currentUserRole);

            var statusExists = await _context.Projectstatuses.AnyAsync(s => s.ProjectStatusId == request.ProjectStatusId);
            if (!statusExists)
                throw new InvalidOperationException("Invalid ProjectStatusId.");

            project.Name = request.Name;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.ProjectStatusId = request.ProjectStatusId;
            project.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(project.Id, currentUserId, currentUserRole);
        
        }

        public async Task DeleteProjectAsync(int projectId, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects.FindAsync(projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            EnsureCanManage(project, currentUserId, currentUserRole);


            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task AddMemberAsync(int projectId, string userId, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            EnsureCanManage(project, currentUserId, currentUserRole);

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new InvalidOperationException("User not found.");

            var alreadyMember = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (alreadyMember)
                throw new InvalidOperationException("User is already a member of this project.");

            _context.ProjectMembers.Add(new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                JoinedDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int projectId, string userId, string currentUserId, string currentUserRole)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            EnsureCanManage(project, currentUserId, currentUserRole);

            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId)
                ?? throw new KeyNotFoundException("This user is not a member of this project.");

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        private static void EnsureCanView(Project project, string currentUserId, string currentUserRole)
        {
            if (currentUserRole == "Administrator")
                return;

            bool isManager = project.ManagerId == currentUserId;
            bool isMember = project.ProjectMembers.Any(pm => pm.UserId == currentUserId);

            if (!isManager && !isMember)
                throw new UnauthorizedAccessException("You do not have access to this project.");
        }

        private static void EnsureCanManage(Project project, string currentUserId, string currentUserRole)
        {
            if (currentUserRole == "Administrator")
                return;

            if (project.ManagerId != currentUserId)
                throw new UnauthorizedAccessException("You are not the manager of this project.");
        }
        private static ProjectResponseDto MapToDto(Project project)
        {
            return new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.ProjectStatus.Description,
                ManagerId = project.ManagerId,
                ManagerName = project.Manager.FullName,
                CreatedDate = project.CreatedDate,
                UpdatedDate = project.UpdatedDate,
                MemberNames = project.ProjectMembers.Select(pm => pm.User.FullName).ToList()
            };
        }
        }
}
