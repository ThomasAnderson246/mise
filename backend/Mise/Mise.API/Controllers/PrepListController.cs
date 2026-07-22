using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PrepListController : ControllerBase
    {
        private readonly IPrepListService _prepListService;
        private readonly ICurrentUserService _currentUser;

        public PrepListController(IPrepListService prepListService, ICurrentUserService currentUser)
        {
            _prepListService = prepListService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [RequiresPermission("preplist", "read")]
        public async Task<IActionResult> GetAll()
        {
            var prepLists = await _prepListService.GetAllAsync(_currentUser.TenantId);
            return Ok(ApiResponse<IEnumerable<PrepListResponse>>.Ok(
                prepLists.Select(pl => MapToResponse(pl))));


        }

        [HttpGet("{id}")]
        [RequiresPermission("preplist", "read")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var prepList = await _prepListService.GetByIdAsync(id, _currentUser.TenantId);
            if (prepList == null)
                return NotFound(ApiResponse<PrepListResponse>.Fail("Prep list not found."));

            return Ok(ApiResponse<PrepListResponse>.Ok(MapToResponse(prepList)));
        }

        [HttpGet("status/{isComplete}")]
        [RequiresPermission("preplist", "read")]
        public async Task<IActionResult> GetByStatus(bool isComplete)
        {
            var prepLists = await _prepListService.GetByStatusAsync(
                _currentUser.TenantId, isComplete);
            return Ok(ApiResponse<IEnumerable<PrepListResponse>>.Ok(prepLists.Select(pl => MapToResponse(pl))));
        }

        [HttpPost]
        [RequiresPermission("preplist", "create")]
        public async Task<IActionResult> Create([FromBody] CreatePrepListRequest request)
        {
            try
            {
                var prepList = await _prepListService.CreateAsync(
                    request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Prep list created."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("preplist", "delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _prepListService.DeleteAsync(id, _currentUser.TenantId, _currentUser.UserId);
                return Ok(ApiResponse<string>.Ok("Deleted.", "prep list deleted."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.Fail("Prep list not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/items")]
        [RequiresPermission("preplist", "update")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddPrepListItemRequest request)
        {
            try
            {
                var prepList = await _prepListService.AddItemAsync(
                    id, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Item added to prep list."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}/items/{itemId}")]
        [RequiresPermission("preplist", "update")]
        public async Task<IActionResult> UpdateItem(
            Guid id, Guid itemId, [FromBody] UpdatePrepListItemRequest request)
        {
            try
            {
                var prepList = await _prepListService.UpdateItemAsync(
                    id, itemId, request, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Item updated."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        [RequiresPermission("preplist", "update")]
        public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
        {
            try
            {
                var prepList = await _prepListService.RemoveItemAsync(
                    id, itemId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Item Removed."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/items/{itemId}/complete")]
        [RequiresPermission("preplist", "update")]
        public async Task<IActionResult> CompleteItem(Guid id, Guid itemId)
        {
            try
            {
                var prepList = await _prepListService.CompleteItemAsync(
                    id, itemId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Item marked complete."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/complete")]
        [RequiresPermission("preplist", "update")]
        public async Task<IActionResult> CompletePrepList(Guid id)
        {
            try
            {
                var prepList = await _prepListService.CompletePrepListAsync(
                    id, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Prep list completed."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail("prep list not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/items/{itemId}/force-complete")]
        [RequiresPermission("preplist", "manage")]
        public async Task<IActionResult> ForceCompleteItem(Guid id, Guid itemId)
        {
            try
            {
                var prepList = await _prepListService.ForceCompleteItemAsync(
                    id, itemId, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Item marked complete."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("{id}/force-complete")]
        [RequiresPermission("preplist", "manage")]
        public async Task<IActionResult> ForceCompletePrepList(Guid id)
        {
            try
            {
                var prepList = await _prepListService.ForceCompletePrepListAsync(
                    id, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Prep list ompleted."));

            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail("Prep list not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }

        [HttpGet("summary")]
        [RequiresPermission("preplist", "read")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _prepListService.GetSummaryAsync(_currentUser.TenantId);
            return Ok(ApiResponse<IEnumerable<PrepListSummaryResponse>>.Ok(summary));
        }

        [HttpPost("{id}/assign")]
        [RequiresPermission("preplist", "manage")]
        public async Task<IActionResult> Assign(Guid id, [FromBody] AssignPrepListRequest request)
        {
            try
            {
                var prepList = await _prepListService.AssignPrepListAsync(
                    id, request.AssignedTo, _currentUser.TenantId, _currentUser.UserId);

                return Ok(ApiResponse<PrepListResponse>.Ok(
                    MapToResponse(prepList), "Prep list assigned."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PrepListResponse>.Fail(ex.Message));
            }
        }




        private static PrepListResponse MapToResponse(PrepList pl) => new()
        {
            PrepListId = pl.PrepListId,
            TenantId = pl.TenantId,
            Name = pl.Name,
            CreatedBy = pl.CreatedBy,
            CreatedByName = pl.CreatedByUser != null
                ? $"{pl.CreatedByUser.FirstName} {pl.CreatedByUser.LastName}"
                : null,
            IsComplete = pl.IsComplete,
            CompletedAt = pl.CompletedAt,
            CreatedAt = pl.CreatedAt,
            AssignedTo = pl.AssignedTo,
            AssignedToName = pl.AssignedToUser != null
                ? $"{pl.AssignedToUser.FirstName} {pl.AssignedToUser.LastName}"
                : null,
            Items = pl.Items.Select(i => new PrepListItemResponse
            {
                PrepListItemId = i.PrepListItemId,
                PrepListId = i.PrepListId,
                RecipeId = i.RecipeId,
                RecipeTitle = i.Recipe.Title,
                DisplayOrder = i.DisplayOrder,
                ScalingFactor = i.ScalingFactor,
                IsComplete = i.IsComplete,
                CompletedBy = i.CompletedBy,
                CompletedByName = i.CompletedByUser != null
                    ? $"{i.CompletedByUser.FirstName} {i.CompletedByUser.LastName}"
                    : null,
                CompletedAt = i.CompletedAt
            }).OrderBy(i => i.DisplayOrder).ToList()
        };
    }
}
