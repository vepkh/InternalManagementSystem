using InternalManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using InternalManagementSystem.Application.Interfaces;
using InternalManagementSystem.Application.DTOs.Users;
using System.Runtime.InteropServices;

namespace InternalManagementSystem.Application.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserResponseDto>();

            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserResponseDto
                {
                    Id = user.Id,                                 
                    FullName = user.FullName,                     
                    Email = user.Email!,                            
                    Role = roles.FirstOrDefault() ?? "Employee",    
                    IsActive = user.IsActive                        
                });
            }

            return result;
        }
        public async Task<UserResponseDto> UpdateUserRoleAsync(string userId, string newRole)
        {

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            if (!await _roleManager.RoleExistsAsync(newRole))
                throw new InvalidOperationException($"Role {newRole} doesn't exist");


            var currentroles = await _userManager.GetRolesAsync(user);

            if (currentroles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentroles);


            await _userManager.AddToRoleAsync(user, newRole);


            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = newRole,         
                IsActive = user.IsActive
            };

        }

        public async Task<UserResponseDto> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

         
            if (!user.IsActive)
                throw new InvalidOperationException("User is already deactivated.");

            user.IsActive = false;
            user.UpdatedDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);// changes saves in sql database

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Employee",
                IsActive = user.IsActive
            };
        }
        public async Task<UserResponseDto> ReactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            if (user.IsActive)
                throw new InvalidOperationException("User is already active.");

            user.IsActive = true;
            user.UpdatedDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Employee",
                IsActive = user.IsActive
            };
        }


    }
}
