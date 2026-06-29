using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.API.Middleware;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantRepositoryService _tenantRepository;
        private readonly ILogger<TenantController> _logger;

        public TenantController(
            ITenantRepositoryService tenantRepository, ILogger<TenantController> logger)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);

            if (tenant == null || !tenant.IsActive)
                return NotFound(ApiResponse<TenantResponse>.Fail("Tenant not found."));

            var response = new TenantResponse
            {
                TenantId = tenant.TenantId,
                Name = tenant.Name,
                Slug = tenant.Slug,
                LogoUrl = tenant.LogoUrl,
                PrimaryColour = tenant.PrimaryColour,
                SecondaryColour = tenant.SecondaryColour,
            };

            return Ok(ApiResponse<TenantResponse>.Ok(response));
        }
    }
}
