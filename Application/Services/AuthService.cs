using InternalManagementSystem.Application.DTOs.Auth;
using InternalManagementSystem.Application.Interfaces;
using InternalManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace InternalManagementSystem.Application.Services
    {
    public class AuthService:IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;//biult-in
        private readonly SignInManager<ApplicationUser> _signInManager;//built-in
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;//built-in
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IHttpContextAccessor httpContextAccessor,
        IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if(existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                FullName = registerRequestDto.FullName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User registration failed: {errors}");
            }
            await _userManager.AddToRoleAsync(user, "Employee");//give user role as employee by default

            await IssueTokensAsync(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Role="Employee"
            };


        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");


            if(!user.IsActive)
                throw new UnauthorizedAccessException("This account has been deactivated.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var primaryrole = roles.FirstOrDefault() ?? "Employee";

            await IssueTokensAsync(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = primaryrole
            };

        }


        private async Task IssueTokensAsync(ApplicationUser user)
        {
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();
            var accessTokenExpiry = _jwtService.GetAccessTokenExpiry();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Persist the refresh token so we can validate/revoke it later
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresDate = refreshTokenExpiry,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            var response = _httpContextAccessor.HttpContext!.Response;

            response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = accessTokenExpiry
            });

            response.Cookies.Append("refreshToken", refreshTokenValue, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenExpiry
            });
        }

        public async Task<AuthResponseDto> RefreshTokenAsync()
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            if (!request.Cookies.TryGetValue("RefreshToken", out var oldRefreshToken) || oldRefreshToken == null)
                throw new UnauthorizedAccessException("No refresh token found.");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(oldRefreshToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (storedToken.IsRevoked)
                throw new UnauthorizedAccessException("This refresh token has been revoked.");

            if (storedToken.ExpiresDate < DateTime.UtcNow)
                throw new UnauthorizedAccessException("This refresh token has expired.");

            // Rotate: revoke the old token so it can't be reused
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.SaveChangesAsync();

            var user = storedToken.User;

            if (!user.IsActive)
                throw new UnauthorizedAccessException("This account has been deactivated.");

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Employee";

            await IssueTokensAsync(user);

            return new AuthResponseDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Role = primaryRole
            };
        }

        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext!;

            if (httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken) && refreshToken != null)
            {
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
                if (storedToken != null)
                {
                    storedToken.IsRevoked = true;
                    await _refreshTokenRepository.SaveChangesAsync();
                }
            }

            httpContext.Response.Cookies.Delete("AccessToken");
            httpContext.Response.Cookies.Delete("RefreshToken");
        }

    }
}
