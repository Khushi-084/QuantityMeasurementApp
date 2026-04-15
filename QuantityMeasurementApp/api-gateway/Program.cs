using System.Net.Http.Headers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var config  = builder.Configuration;

// ── Service URLs ───────────────────────────────────────────────────────────
var authServiceUrl = config["Services:AuthService"] ?? "http://localhost:5001";
var qmaServiceUrl  = config["Services:QmaService"]  ?? "http://localhost:5002";

// ── HTTP clients ───────────────────────────────────────────────────────────
builder.Services.AddHttpClient("auth-service", c => c.BaseAddress = new Uri(authServiceUrl));
builder.Services.AddHttpClient("qma-service",  c => c.BaseAddress = new Uri(qmaServiceUrl));

// ── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ── OpenAPI (replaces Swashbuckle) ─────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title       = "QMA API Gateway";
        document.Info.Version     = "v1";
        document.Info.Description = "Single entry point — routes to auth-service (port 5001) and qma-service (port 5002)";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// ── Scalar UI at /scalar/v1 (replaces Swagger UI) ─────────────────────────
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "QMA API Gateway";
});

app.UseCors("FrontendPolicy");

// ═══════════════════════════════════════════════════════════════════════════
// HELPER — forward request to downstream, relay response
// ═══════════════════════════════════════════════════════════════════════════
static async Task ForwardAsync(HttpContext ctx, IHttpClientFactory factory,
    string clientName, string downstreamPath)
{
    var client  = factory.CreateClient(clientName);
    var method  = new HttpMethod(ctx.Request.Method);
    var request = new HttpRequestMessage(method, downstreamPath);

    if (ctx.Request.Headers.TryGetValue("Authorization", out var auth))
    {
        if (AuthenticationHeaderValue.TryParse(auth.ToString(), out var authHeader))
            request.Headers.Authorization = authHeader;
    }

    if (ctx.Request.ContentLength > 0 || ctx.Request.Headers.ContainsKey("Transfer-Encoding"))
    {
        var bodyBytes = new MemoryStream();
        await ctx.Request.Body.CopyToAsync(bodyBytes);
        bodyBytes.Seek(0, SeekOrigin.Begin);
        request.Content = new StreamContent(bodyBytes);
        
        if (MediaTypeHeaderValue.TryParse(ctx.Request.ContentType ?? "application/json", out var contentType))
            request.Content.Headers.ContentType = contentType;
    }

    try
    {
        var response = await client.SendAsync(request);
        ctx.Response.StatusCode  = (int)response.StatusCode;
        ctx.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
        await response.Content.CopyToAsync(ctx.Response.Body);
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 502;
        await ctx.Response.WriteAsJsonAsync(new { error = "Gateway error", message = ex.Message });
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// ROUTES: /api/v1/users  →  auth-service
// ═══════════════════════════════════════════════════════════════════════════
var users = app.MapGroup("/api/v1/users");

users.MapPost("/signup",       (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/signup"))
    .WithName("Signup").WithTags("Auth");

users.MapPost("/login",        (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/login"))
    .WithName("Login").WithTags("Auth");

users.MapPost("/google-login", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/google-login"))
    .WithName("GoogleLogin").WithTags("Auth");

users.MapGet("/profile",       (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/profile"))
    .WithName("Profile").WithTags("Auth");

// ═══════════════════════════════════════════════════════════════════════════
// ROUTES: /api/v1/quantities  →  qma-service
// ═══════════════════════════════════════════════════════════════════════════
var qty = app.MapGroup("/api/v1/quantities");

qty.MapPost("/compare",  (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/compare"))
    .WithName("Compare").WithTags("Quantities");

qty.MapPost("/convert",  (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/convert"))
    .WithName("Convert").WithTags("Quantities");

qty.MapPost("/add",      (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/add"))
    .WithName("Add").WithTags("Quantities");

qty.MapPost("/subtract", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/subtract"))
    .WithName("Subtract").WithTags("Quantities");

qty.MapPost("/divide",   (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/divide"))
    .WithName("Divide").WithTags("Quantities");

qty.MapGet("/history/all", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/history/all"))
    .WithName("HistoryAll").WithTags("History");

qty.MapGet("/history/operation/{operationType}", (HttpContext ctx, IHttpClientFactory f, string operationType) =>
    ForwardAsync(ctx, f, "qma-service", $"/api/v1/quantities/history/operation/{operationType}"))
    .WithName("HistoryByOperation").WithTags("History");

qty.MapGet("/history/category/{category}", (HttpContext ctx, IHttpClientFactory f, string category) =>
    ForwardAsync(ctx, f, "qma-service", $"/api/v1/quantities/history/category/{category}"))
    .WithName("HistoryByCategory").WithTags("History");

qty.MapGet("/history/errors", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/history/errors"))
    .WithName("HistoryErrors").WithTags("History");

qty.MapGet("/count/{operationType}", (HttpContext ctx, IHttpClientFactory f, string operationType) =>
    ForwardAsync(ctx, f, "qma-service", $"/api/v1/quantities/count/{operationType}"))
    .WithName("OperationCount").WithTags("History");

// ── Health check ───────────────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new
{
    service = "api-gateway",
    status  = "healthy",
    routes  = new { authService = authServiceUrl, qmaService = qmaServiceUrl }
}));

app.Run();