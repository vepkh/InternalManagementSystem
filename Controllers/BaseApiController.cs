using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace InternalManagementSystem.Controllers;

public abstract class BaseApiController : ControllerBase
{
 
    protected string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User Id claim not found.");
    }

    protected string GetCurrentUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException("Role claim not found.");
    }
}