using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AllergenTagController : ControllerBase
    {


        private readonly ICurrentUserService _currentUser;
        private readonly IAllergenTagService _allergenTagService;

        public AllergenTagController(IAllergenTagService allergenTagService, ICurrentUserService currentUser)
        {
            _allergenTagService = allergenTagService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [RequiresPermission("allergen", "read")]
        public async Task<IActionResult> GetAll()
        {
            var allergens = await _allergenTagService.GetAllAsync(_currentUser.TenantId);

            return Ok(ApiResponse<IEnumerable<AllergenTagResponse>>.Ok(allergens.Select(a => MapToResponse(a))));
        }

        [HttpGet("{id}")]
        [RequiresPermission("allergen", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var allergen = await _allergenTagService.GetByIdAsync(id, _currentUser.TenantId);
            if (allergen == null)
                return NotFound(ApiResponse<AllergenTagResponse>.Fail("Allergen tag not found."));

            return Ok(ApiResponse<AllergenTagResponse>.Ok(MapToResponse(allergen)));
        }

        [HttpPost]
        [RequiresPermission("allergen", "create")]
        public async Task<IActionResult> Create([FromBody] CreateAllergenTagRequest request)
        {
            try
            {
                var allergen = await _allergenTagService.CreateAsync(request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<AllergenTagResponse>.Ok(MapToResponse(allergen), "Allergen tag created."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AllergenTagResponse>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [RequiresPermission("allergen", "update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAllergenTagRequest request)
        {
            try
            {
                var allergen = await _allergenTagService.UpdateAsync(
                    id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<AllergenTagResponse>.Ok(
                    MapToResponse(allergen), "Allergen tag updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<AllergenTagResponse>.Fail("Allergen tag not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AllergenTagResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete]
        [RequiresPermission("allergen", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _allergenTagService.DeleteAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "Allergen tag deleted."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Allergen tag not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        private static AllergenTagResponse MapToResponse(AllergenTag a) => new()
        {
            AllergenId = a.AllergenId,
            Name = a.Name,
            Description = a.Description,
            IsMajor = a.IsMajor,
            IsSystemDefined = a.IsSystemDefined
        };
    }
}
