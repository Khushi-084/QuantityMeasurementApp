using QuantityMeasurementBusinessLayer.Interface;
using System.Security.Claims;

namespace QuantityMeasurementApi.Middleware
{
    /// <summary>
    /// UC17: JWT extraction middleware.
    /// Reads Bearer token from Authorization header, validates it via IJwtService,
    /// then populates HttpContext.User so [Authorize] and User.FindFirstValue() work.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        public JwtMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var header = context.Request.Headers.Authorization.FirstOrDefault();
            if (header is not null && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = header["Bearer ".Length..].Trim();
                var (isValid, userId, email, role) = jwtService.ValidateToken(token);
                if (isValid)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Email,          email),
                        new Claim(ClaimTypes.Role,           role),
                        new Claim("UserId",                  userId.ToString())
                    };
                    context.User  = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                    context.Items["UserId"] = userId;
                }
            }
            await _next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtExtraction(this IApplicationBuilder app)
            => app.UseMiddleware<JwtMiddleware>();
    }
}
