using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ICurrentUserService _currentUser;

        public RoleController(
            IRoleService roleService, ICurrentUserService currentUser)
        {
            _roleService = roleService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [RequiresPermission("user", "manage")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync(_currentUser.TenantId);
            return Ok(ApiResponse<IEnumerable<RoleResponse>>.Ok(roles.Select(r => MapToResponse(r))));
        }

        [HttpGet("{id}")]
        [RequiresPermission("user","manage")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id, _currentUser.TenantId);
            if (role == null)
                return NotFound(ApiResponse<RoleResponse>.Fail("Role not found."));

            return Ok(ApiResponse<RoleResponse>.Ok(MapToResponse(role)));
        }

        [HttpPost]
        [RequiresPermission("user","manage")]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            try
            {
                var role = await _roleService.CreateAsync(
                    request,
                    _currentUser.TenantId,
                    _currentUser.UserId);

                return Ok(ApiResponse<RoleResponse>.Ok(
                    MapToResponse(role), "Role created."));
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RoleResponse>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [RequiresPermission("user", "manage")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                var role = await _roleService.UpdateAsync(
                    id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<RoleResponse>.Ok(
                    MapToResponse(role), "Role updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<RoleResponse>.Fail("role not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RoleResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("user", "manage")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _roleService.DeleteAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "Role deleted."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Role not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/permissions")]
        [RequiresPermission("user", "manage")]
        public async Task<IActionResult> AssignPermission(Guid id, [FromBody] RolePermissionRequest request)
        {
            try
            {
                await _roleService.AssignPermissionAsync(
                    id, request.PermissionId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<string>.Ok("Permission assigned.", "Permission assigned to role."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/permissions/{permissionId}")]
        [RequiresPermission("user", "manage")]
        public async Task<IActionResult> RemovePermission(Guid id, Guid permissionId)
        {
            try
            {
                await _roleService.RemovePermissionAsync(
                    id, permissionId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<string>.Ok("Permission removed.", "Permission removed from role."));
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet("/api/permission")]
        [RequiresPermission("user", "manage")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _roleService.GetAllPermissionsAsync();
            return Ok(ApiResponse<IEnumerable<object>>.Ok(
                permissions.Select(p => new
                {
                    PermissionId = p.PermissionId,
                    Name = p.Name,
                    Resource = p.Resource,
                    Action = p.Action,
                    Description = p.Description
                })));
        }

        private static RoleResponse MapToResponse(Role r) => new()
        {
            RoleId = r.RoleId,
            TenantId = r.TenantId,
            Name = r.Name,
            IsSystemRole = r.IsSystemRole,
            Permissions = r.RolePermissions.Select(rp => new RolePermissionResponse
            {
                PermissionId = rp.Permission.PermissionId,
                Name = rp.Permission.Name,
                Resource = rp.Permission.Resource,
                Action = rp.Permission.Action
            }).ToList(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}
