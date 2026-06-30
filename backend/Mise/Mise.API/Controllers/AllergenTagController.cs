using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class AllergenTagController : ControllerBase
    {

        private readonly MiseDbContext _context;

        public AllergenTagController(MiseDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allergens = await _context.AllergenTags
                .OrderBy(a => a.Name)
                .ToListAsync();

            var response = allergens.Select(a => new AllergenTagResponse
            {
                AllergenId = a.AllergenId,
                Name = a.Name,
                Description = a.Description,
                IsMajor = a.IsMajor,
            });

            return Ok(ApiResponse<IEnumerable<AllergenTagResponse>>.Ok(response));
        }
    }
}
