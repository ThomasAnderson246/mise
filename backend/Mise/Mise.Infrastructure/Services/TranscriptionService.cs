using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.EntityFrameworkCore;
using Mise.Application.DTOs;
using Mise.Application.Interfaces;
using Mise.Domain.Entities;
using Mise.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Anthropic.SDK.Constants;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Mise.Infrastructure.Services
{
    public class TranscriptionService : ITranscriptionService
    {

        private readonly MiseDbContext _context;
        private readonly AnthropicClient _anthropic;
        private readonly ILogger<TranscriptionService> _logger;

        public TranscriptionService(MiseDbContext context, AnthropicClient anthropic, ILogger<TranscriptionService> logger)
        {
            _context = context;
            _anthropic = anthropic;
            _logger = logger;
        }

        public async Task<TranscriptionResponse> TranscribeAsync(
            TranscriptionRequest request,
            Guid tenantId,
            Guid uploadedBy)
        {
            var job = new TranscriptionJob
            {
                JobId = Guid.NewGuid(),
                TenantId = tenantId,
                UploadedBy = uploadedBy,
                Status = "processing",
                CreatedAt = DateTime.UtcNow,
            };

            await _context.TranscriptionJobs.AddAsync(job);
            await _context.SaveChangesAsync();

            try
            {
                var prompt = @"You are a professional chef's assistant. 
Analyze this recipe image and extract the recipe information.
Respond ONLY with a valid JSON object in this exact format, no other text, no markdown, no code fences:
{
  ""title"": ""Recipe Title"",
  ""description"": ""Brief description"",
  ""confidenceScore"": 0.95,
  ""ingredients"": [
    {
      ""name"": ""ingredient name"",
      ""quantity"": 1.0,
      ""unit"": ""cup"",
      ""notes"": ""optional notes""
    }
  ],
  ""steps"": [
    {
      ""stepNumber"": 1,
      ""instruction"": ""Step instruction"",
      ""hasTimer"": false,
      ""timerDuration"": null
    },
    {
      ""stepNumber"": 2,
      ""instruction"": ""Step with timer example"",
      ""hasTimer"": true,
      ""timerDuration"": 3
    }
  ]
}
Important rules:
- timerDuration must be an integer number of minutes or null, never a string. Example: 3 not '3 minutes'
- quantity must be a number, never a string
- confidenceScore must be between 0 and 1
- If you cannot read the image clearly, set confidenceScore below 0.7
- Return raw JSON only, no markdown formatting, no code fences";

                var messages = new List<Message>
                {
                    new Message
                    {
                        Role = RoleType.User,
                        Content = new List<ContentBase>
                        {
                            new ImageContent
                            {
                                Source = new ImageSource
                                {
                                    Type = SourceType.base64,
                                    MediaType = request.MediaType,
                                    Data = request.ImageBase64
                                }
                            },
                            new TextContent
                            {
                                Text = prompt
                            }
                        }
                    }
                };

                var response = await _anthropic.Messages.GetClaudeMessageAsync(
                    new MessageParameters
                    {
                        Model = AnthropicModels.Claude45Sonnet,
                        MaxTokens = 2048,
                        Messages = messages
                    });

                var rawResponse = response.Content
                    .OfType<TextContent>()
                    .FirstOrDefault()?.Text ?? string.Empty;

                _logger.LogInformation("Claude raw response: {Response}", rawResponse);

                var jsonToParse = rawResponse.Replace("```json", "").Replace("```", "").Trim();




                TranscriptionResultData? parsedData = null;

                try
                {
                    parsedData = JsonSerializer.Deserialize<TranscriptionResultData>(
                       jsonToParse,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse transcription response as JSON.");
                }

                var result = new TranscriptionResult
                {
                    ResultId = Guid.NewGuid(),
                    JobId = job.JobId,
                    RawResponse = rawResponse,
                    ParsedTitle = parsedData?.Title,
                    ConfidenceScore = parsedData?.ConfidenceScore,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.TranscriptionResults.AddAsync(result);

                job.Status = "completed";
                job.CompletedAt = DateTime.UtcNow;
                _context.TranscriptionJobs.Update(job);

                await _context.SaveChangesAsync();

                return MapToResponse(job, result, parsedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transcription failed for job {JobId}", job.JobId);

                job.Status = "failed";
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                _context.TranscriptionJobs.Update(job);
                await _context.SaveChangesAsync();

                return MapToResponse(job, null, null);
            }
        }

        public async Task<TranscriptionResponse?> GetJobAsync(Guid jobId, Guid tenantId)
        {
            var job = await _context.TranscriptionJobs
                .Include(tj => tj.Result)
                .FirstOrDefaultAsync(tj => tj.JobId == jobId && tj.TenantId == tenantId);

            if (job == null) return null;

            TranscriptionResultData? parsedData = null;
            if (job.Result?.RawResponse != null)
            {
                try
                {
                    parsedData = JsonSerializer.Deserialize<TranscriptionResultData>(
                        job.Result.RawResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { }
            }

            return MapToResponse(job, job.Result, parsedData);
        }

        public async Task<IEnumerable<TranscriptionResponse>> GetJobsByTenantAsync(Guid tenantId)
        {
            var jobs = await _context.TranscriptionJobs
                .Where(tj => tj.TenantId == tenantId)
                .Include(tj => tj.Result)
                .OrderByDescending(tj => tj.CreatedAt)
                .ToListAsync();

            return jobs.Select(job =>
            {
                TranscriptionResultData? parsedData = null;
                if (job.Result?.RawResponse != null)
                {
                    try
                    {
                        parsedData = JsonSerializer.Deserialize<TranscriptionResultData>(
                            job.Result.RawResponse,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch { }
                }

                return MapToResponse(job, job.Result, parsedData);
            });
        }

        private static TranscriptionResponse MapToResponse(
            TranscriptionJob job,
            TranscriptionResult? result,
            TranscriptionResultData? parsedData) => new()
            {
                JobId = job.JobId,
                Status = job.Status,
                ErrorMessage = job.ErrorMessage,
                CreatedAt = job.CreatedAt,
                CompletedAt = job.CompletedAt,
                Result = parsedData == null ? null : new TranscriptionResultData
                {
                    Title = parsedData.Title,
                    Description = parsedData.Description,
                    Ingredients = parsedData.Ingredients,
                    Steps = parsedData.Steps,
                    ConfidenceScore = parsedData.ConfidenceScore,
                    RawResponse = result?.RawResponse
                }
            };
    }
}
