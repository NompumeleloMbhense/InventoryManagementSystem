using System.Net;
using System.Text.Json;
using FluentValidation;
using Serilog;


/// <summary>
/// Global exception handling middleware for centralized error management
/// Catches exceptions thrown during reques processing and returns standardized 
/// error responses 
/// </summary>

namespace ServerApp.Middleware
{

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        // Centalized exception handling logic with specific handling for common exception types
        // Returns appropriate HTTP status codes and error messages based on the exception type
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Default to 500 Internal Server Error
            var statusCode = HttpStatusCode.InternalServerError;
            object responsePayload;

            // Determine the Status Code based on the Exception Type
            switch (exception)
            {
                case ValidationException valEx:
                    statusCode = HttpStatusCode.BadRequest;
                    responsePayload = new
                    {
                        error = "Validation Failed",
                        details = valEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    };
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    responsePayload = new { error = exception.Message };
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    responsePayload = new { error = exception.Message };
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    responsePayload = new { error = "You are not authorized to perform this action." };
                    break;

                default:
                    // Log the full stack trace for internal server errors
                    Log.Error(exception, "An unhandled exception occurred: {Message}", exception.Message);
                    responsePayload = new { error = "An unexpected server error occurred. Please try again later." };
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload, jsonOptions));
        }
    }
}