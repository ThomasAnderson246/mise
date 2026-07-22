//using Microsoft.AspNetCore.Mvc;
//using Mise.API;
//using Mise.Application.DTOs;
//using Mise.Application.Interfaces;
//using Mise.Domain.Entities;

//namespace Mise.API.Controllers
//{
//    [ApiController]
//    [Route("/api/[controller]")]
//    public class UserController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly ICurrentUserService _currentUserService;

//        public UserController(IUserService userService, ICurrentUserService currentUserService)
//        {
//            _userService = userService;
//            _currentUserService = currentUserService;
//        }

//        [HttpGet]
//        [RequiresPermission("user", "manage")]
//        public async Task<IActionResult> GetAll()
//        {
//            var users = await _userService.GetAllAsync(_currentUserService.TenantId);
//            return Ok(ApiResponse<IEnumerable<UserResponse>>.Ok(
//                users.Select(u => MapToResponse(u))));
//        }

//        [HttpGet("{id}")]
//        [RequiresPermission("user","manage")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var user = await _userService.GetByIdAsync(id, _currentUserService.TenantId);
//            if (user == null)
//                return NotFound(ApiResponse<UserResponse>.Fail("User not found."));

//            return Ok(ApiResponse<UserResponse>.Ok(MapToResponse(user)));
//        }

//        [HttpPost("invite")]
//        [RequiresPermission("user","manage")]
//        public async Task<IActionResult> Invite([FromBody] InviteUserRequest request)
//        {
//            try
//            {
//                var (user, tempPassword) = await _userService.InviteAsync(request, _currentUserService.TenantId, _currentUserService.UserId);

//                return Ok(ApiResponse<InviteUserResponse>.Ok(new InviteUserResponse
//                {
//                    User = MapToResponse(user),
//                    TemporaryPassword = tempPassword
//                }, "User invited successfully."));
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ApiResponse<InviteUserResponse>.Fail(ex.Message));
//            }
//        }

//        [HttpPut("{id}")]
//        [RequiresPermission("user","manage")]
//        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
//        {
//            try
//            {
//                var user = await _userService.UpdateAsync(
//                    id, request, _currentUserService.TenantId, _currentUserService.UserId);

//                return Ok(ApiResponse<UserResponse>.Ok(
//                    MapToResponse(user), "User updated."));
//            }
//            catch (KeyNotFoundException)
//            {
//                return NotFound(ApiResponse<UserResponse>.Fail("User not found."));
//            }
//        }

//        [HttpPost("{id}/deactivate")]
//        [RequiresPermission("user","manage")]
//        public async Task<IActionResult> Deactivate(Guid id)
//        {
//            try
//            {
//                await _userService.DeactivateAsync(id, _currentUserService.TenantId, _currentUserService.UserId);
//                return Ok(ApiResponse<string>.Ok("Deactivate.", "User deactivate."));
//            }
//            catch (KeyNotFoundException)
//            {
//                return NotFound(ApiResponse<string>.Fail("User not found."));
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ApiResponse<string>.Fail(ex.Message));
//            }
//        }

//        [HttpPost("{id}/reactivate")]
//        [RequiresPermission("user", "manage")]
//        public async Task<IActionResult> Reactivate(Guid id)
//        {
//            try
//            {
//                await _userService.ReactivateAsync(id, _currentUserService.TenantId, _currentUserService.UserId);
//                return Ok(ApiResponse<string>.Ok("Reactivate.", "User reactivated."));
//            }
//            catch (KeyNotFoundException)
//            {
//                return NotFound(ApiResponse<string>.Fail("User not found."));
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ApiResponse<string>.Fail(ex.Message));
//            }
//        }

//        [HttpPost("{id}/roles")]
//        [RequiresPermission("user", "manage")]
//        public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleRequest request)
//        {
//            try
//            {
//                await _userService.AssignRoleAsync(
//                    id,
//                    request.RoleId,
//                    _currentUserService.TenantId,
//                    _currentUserService.UserId);
//                return Ok(ApiResponse<string>.Ok("Role assigned.", "Role assigned successfully."));
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ApiResponse<string>.Fail(ex.Message));
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ApiResponse<string>.Fail(ex.Message));
//            }
//        }

//        [HttpDelete("{id}/roles/{roleId}")]
//        [RequiresPermission("user", "manage")]
//        public async Task<IActionResult> RemoveRole(Guid id, Guid roleId)
//        {
//            try
//            {
//                await _userService.RemoveRoleAsync(
//                    id,
//                    roleId,
//                    _currentUserService.TenantId,
//                    _currentUserService.UserId);

//                return Ok(ApiResponse<string>.Ok("Role removed.", "Role removed successfully."));
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ApiResponse<string>.Fail(ex.Message));
//            }
//        }

//        private static UserResponse MapToResponse(User u) => new()
//        {
//            UserId = u.UserId,
//            TenantId = u.TenantId,
//            Email = u.Email,
//            FirstName = u.FirstName,
//            LastName = u.LastName,
//            Status = u.Status,
//            UnitPreference = u.UnitPreference,
//            MustChangePassword = u.MustChangePassword,
//            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
//            LastLoginAt = u.LastLoginAt,
//            CreatedAt = u.CreatedAt,
//            UpdatedAt = u.UpdatedAt
//        };
//    }
//}
