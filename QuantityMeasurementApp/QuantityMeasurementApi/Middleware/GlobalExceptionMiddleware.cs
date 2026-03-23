using QuantityMeasurementBusinessLayer;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementModel.Dto;
using System.Net;
using System.Text.Json;

namespace QuantityMeasurementApi.Middleware
{
    /// <summary>
    /// UC17: Centralized exception handling middleware.
    /// Registered first in Program.cs — wraps every controller and middleware beneath it.
    /// Converts all unhandled exceptions into structured JSON ErrorResponseDTO.
    ///
    /// Mapping:
    ///   QuantityMeasurementException → 400 Bad Request
    ///   AuthException (401/403/409)  → corresponding status
    ///   NotFoundException            → 404 Not Found
    ///   DivideByZeroException        → 400 Bad Request
    ///   Everything else              → 500 Internal Server Error
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions _opts =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await WriteErrorAsync(context, ex);
            }
        }

        private static async Task WriteErrorAsync(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";

            var err = ex switch
            {
                QuantityMeasurementException qme => new ErrorResponseDTO
                {
                    Status = 400, Error = "Quantity Measurement Error",
                    Message = qme.Message, Path = ctx.Request.Path
                },
                AuthException ae => new ErrorResponseDTO
                {
                    Status  = ae.StatusCode,
                    Error   = ae.StatusCode switch { 401=>"Unauthorized", 403=>"Forbidden", 409=>"Conflict", _=>"Auth Error" },
                    Message = ae.Message, Path = ctx.Request.Path
                },
                NotFoundException nfe => new ErrorResponseDTO
                {
                    Status = 404, Error = "Not Found",
                    Message = nfe.Message, Path = ctx.Request.Path
                },
                DivideByZeroException dze => new ErrorResponseDTO
                {
                    Status = 400, Error = "Bad Request",
                    Message = dze.Message, Path = ctx.Request.Path
                },
                _ => new ErrorResponseDTO
                {
                    Status = 500, Error = "Internal Server Error",
                    Message = "An unexpected error occurred.", Path = ctx.Request.Path
                }
            };

            ctx.Response.StatusCode = err.Status;
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(err, _opts));
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}