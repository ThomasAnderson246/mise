using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController (IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<LoginResponse>.Fail("Invalid Request."));

            var user = await _authService.ValidateUserAsync(
                request.Email,
                request.Password,
                request.TenantId
                );

            if (user == null)
                return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid email or password."));

            var roleName = await _authService.GetUserRoleAsync(user.UserId);

            var token = await _authService.GenerateTokenAsync(user, roleName);

            try
            {
                var refreshToken = await _authService.GenerateRefreshTokenAsync(user.UserId, user.TenantId);

                Response.Cookies.Append("refreshtoken", refreshToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshToken.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate fresh token");
                return StatusCode(500, ApiResponse<LoginResponse>.Fail("Failed to generate refresh token."));
            }
            var response = new LoginResponse
            {
                Token = token,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roleName,
                TenantId = user.TenantId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            return Ok(ApiResponse<LoginResponse>.Ok(response, "Login Successful."));
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            // get refresh token from http-only cookie
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(ApiResponse<LoginResponse>.Fail("No refresh token provided."));

            var (user, roleName) = await _authService.ValidateRefreshTokenAsync(refreshToken, request.TenantId);

            if (user == null)
                return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid or expired token"));

            //rotate the token
            var newRefreshToken = await _authService.GenerateRefreshTokenAsync(user.UserId, user.TenantId);
            var newAccessToken = await _authService.GenerateTokenAsync(user, roleName);

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.ExpiresAt

            });

            var response = new LoginResponse
            {
                Token = newAccessToken,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roleName,
                TenantId = user.TenantId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            return Ok(ApiResponse<LoginResponse>.Ok(response, "Token refresh successful."));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
                await _authService.RevokeRefreshTokenAsync(refreshToken);

            //clear the cookie
            Response.Cookies.Delete("refreshToken");

            return Ok(ApiResponse<string>.Ok("Logged out.", "Logout successful."));
        }
    }
}
