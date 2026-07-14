using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UnitTypeController : ControllerBase
    {
        private readonly IUnitTypeService _unitTypeService;
        private readonly ICurrentUserService _currentUser;

        public UnitTypeController(IUnitTypeService unitTypeService, ICurrentUserService currentUser)
        {
            _unitTypeService = unitTypeService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [RequiresPermission("unit", "read")]
        public async Task<IActionResult> GetAll()
        {
            var unitTypes = await _unitTypeService.GetAllAsync(_currentUser.TenantId);
            return Ok(ApiResponse<IEnumerable<UnitTypeResponse>>.Ok(
                unitTypes.Select(u => MapToResponse(u))));
        }

        [HttpGet("{id}")]
        [RequiresPermission("unit", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var unitType = await _unitTypeService.GetByIdAsync(id, _currentUser.TenantId);
            if (unitType == null)
                return NotFound(ApiResponse<UnitTypeResponse>.Fail("Unit type not found."));

            return Ok(ApiResponse<UnitTypeResponse>.Ok(MapToResponse(unitType)));
        }

        [HttpGet("by-measure/{measureType}")]
        [RequiresPermission("unit", "read")]
        public async Task<IActionResult> GetByMeasureType(string measureType)
        {
            var unitTypes = await _unitTypeService.GetByMeasureTypeAsync(_currentUser.TenantId, measureType);
            return Ok(ApiResponse<IEnumerable<UnitTypeResponse>>.Ok(unitTypes.Select(u => MapToResponse(u))));
        }

        [HttpGet("by-system/{system}")]
        [RequiresPermission("unit", "read")]
        public async Task<IActionResult> GetBySystem(string system)
        {
            var unitTypes = await _unitTypeService.GetBySystemAsync(_currentUser.TenantId, system);
            return Ok(ApiResponse<IEnumerable<UnitTypeResponse>>.Ok(unitTypes.Select(u => MapToResponse(u))));
        }

        [HttpPost]
        [RequiresPermission("unit", "create")]
        public async Task<IActionResult> Create([FromBody] CreateUnitTypeRequest request)
        {
            try
            {
                var unitType = await _unitTypeService.CreateAsync(
                    request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<UnitTypeResponse>.Ok(
                    MapToResponse(unitType), "Unit type created."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UnitTypeResponse>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [RequiresPermission("unit", "update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnityTypeRequest request)
        {
            try
            {
                var unitType = await _unitTypeService.UpdateAsync(
                    id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<UnitTypeResponse>.Ok(MapToResponse(unitType), "Unit type updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<UnitTypeResponse>.Fail("Unit type not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UnitTypeResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("unit", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _unitTypeService.DeleteAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "Unit type deleted."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Unit type not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }
        private static UnitTypeResponse MapToResponse(UnitType u) => new()
        {
            UnitTypeId = u.UnitTypeId,
            TenantId = u.TenantId,
            Name = u.Name,
            Abbreviation = u.Abbreviation,
            System = u.System,
            MeasureType = u.MeasureType,
            ConversionFactor = u.ConversionFactor,
            IsNonConvertible = u.IsNonConvertible,
            IsSystemDefined = u.IsSystemDefined
        };
    }
}
