using System.Net;
using System.Text.Json;

namespace Mise.API.Middleware
{
    public class ErrorHandlingMIddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMIddleware> _logger;

        public ErrorHandlingMIddleware(RequestDelegate next, ILogger<ErrorHandlingMIddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                KeyNotFoundException => new
                {
                    success = false,
                    errors = new List<string> { exception.Message },
                    statusCode = (int)HttpStatusCode.NotFound
                },
                UnauthorizedAccessException => new
                {
                    success = false,
                    errors = new List<string> { "Unauthorized." },
                    statusCode = (int)HttpStatusCode.Unauthorized
                },
                ArgumentException => new
                {
                    success = false,
                    errors = new List<string> { exception.Message },
                    statusCode = (int)HttpStatusCode.BadRequest
                },
                _ => new
                {
                    success = false,
                    errors = new List<string> { "An unexpected error occurred." },
                    statusCode = (int)HttpStatusCode.InternalServerError
                }
            };

            context.Response.StatusCode = response.statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}
