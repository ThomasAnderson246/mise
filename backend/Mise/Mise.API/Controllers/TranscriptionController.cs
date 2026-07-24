using Microsoft.AspNetCore.Mvc;
using Mise.API;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;

namespace Mise.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TranscriptionController : ControllerBase
    {
        private readonly ITranscriptionService _transcriptionService;
        private readonly ICurrentUserService _currentUser;

        public TranscriptionController(
            ITranscriptionService transcriptionService, ICurrentUserService currentUser)
        {
            _transcriptionService = transcriptionService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [RequiresPermission("recipe", "create")]
        public async Task<IActionResult> Transcribe([FromBody] TranscriptionRequest request)
        {
            if (string.IsNullOrEmpty(request.ImageBase64))
                return BadRequest(ApiResponse<TranscriptionResponse>.Fail("Image data is required."));

            var result = await _transcriptionService.TranscribeAsync(
                request,
                _currentUser.TenantId,
                _currentUser.UserId);

            return Ok(ApiResponse<TranscriptionResponse>.Ok(result, "Transcription complete."));
        }

        [HttpGet("{jobid}")]
        [RequiresPermission("recipe", "create")]
        public async Task<IActionResult> GetJob(Guid jobId)
        {
            var job = await _transcriptionService.GetJobAsync(jobId, _currentUser.TenantId);
            if (job == null)
                return NotFound(ApiResponse<TranscriptionResponse>.Fail("Job not found."));

            return Ok(ApiResponse<TranscriptionResponse>.Ok(job));
        }

        [HttpGet]
        [RequiresPermission("recipe", "create")]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await _transcriptionService.GetJobsByTenantAsync(_currentUser.TenantId);
            return Ok(ApiResponse<IEnumerable<TranscriptionResponse>>.Ok(jobs));
        }
    }
}
