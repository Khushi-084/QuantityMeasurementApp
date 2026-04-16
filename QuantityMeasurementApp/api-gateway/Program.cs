using System.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ── Service URLs ───────────────────────────────────────────────────────────
var authServiceUrl = config["Services:AuthService"] ?? "http://localhost:5001";
var qmaServiceUrl  = config["Services:QmaService"]  ?? "http://localhost:5002";

// ── HTTP clients ───────────────────────────────────────────────────────────
builder.Services.AddHttpClient("auth-service", c =>
    c.BaseAddress = new Uri(authServiceUrl));

builder.Services.AddHttpClient("qma-service", c =>
    c.BaseAddress = new Uri(qmaServiceUrl));

// ── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ── Swagger ────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QMA API Gateway",
        Version = "v1",
        Description = "Single entry point — routes to auth-service and qma-service"
    });
});

var app = builder.Build();

// ── Swagger middleware ─────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("FrontendPolicy");

// ═══════════════════════════════════════════════════════════════════════════
// HELPER — forward request to downstream service
// ═══════════════════════════════════════════════════════════════════════════
static async Task ForwardAsync(HttpContext ctx, IHttpClientFactory factory,
    string clientName, string downstreamPath)
{
    var client = factory.CreateClient(clientName);
    var method = new HttpMethod(ctx.Request.Method);
    var request = new HttpRequestMessage(method, downstreamPath);

    // Forward Authorization header
    if (ctx.Request.Headers.TryGetValue("Authorization", out var auth))
    {
        if (AuthenticationHeaderValue.TryParse(auth.ToString(), out var authHeader))
        {
            request.Headers.Authorization = authHeader;
        }
    }

    // Forward body if exists
    if (ctx.Request.ContentLength > 0 || ctx.Request.Headers.ContainsKey("Transfer-Encoding"))
    {
        var bodyStream = new MemoryStream();
        await ctx.Request.Body.CopyToAsync(bodyStream);
        bodyStream.Position = 0;

        request.Content = new StreamContent(bodyStream);

        if (MediaTypeHeaderValue.TryParse(ctx.Request.ContentType ?? "application/json",
            out var contentType))
        {
            request.Content.Headers.ContentType = contentType;
        }
    }

    try
    {
        var response = await client.SendAsync(request);

        ctx.Response.StatusCode = (int)response.StatusCode;
        ctx.Response.ContentType = response.Content.Headers.ContentType?.ToString()
                                   ?? "application/json";

        await response.Content.CopyToAsync(ctx.Response.Body);
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 502;
        await ctx.Response.WriteAsJsonAsync(new
        {
            error = "Gateway error",
            message = ex.Message
        });
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// ROUTES: USERS → AUTH SERVICE
// ═══════════════════════════════════════════════════════════════════════════
var users = app.MapGroup("/api/v1/users");

users.MapPost("/signup", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/signup"))
    .WithName("Signup")
    .WithTags("Auth");

users.MapPost("/login", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/login"))
    .WithName("Login")
    .WithTags("Auth");

users.MapPost("/google-login", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/google-login"))
    .WithName("GoogleLogin")
    .WithTags("Auth");

users.MapGet("/profile", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "auth-service", "/api/v1/users/profile"))
    .WithName("Profile")
    .WithTags("Auth");

// ═══════════════════════════════════════════════════════════════════════════
// ROUTES: QUANTITIES → QMA SERVICE
// ═══════════════════════════════════════════════════════════════════════════
var qty = app.MapGroup("/api/v1/quantities");

qty.MapPost("/compare", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/compare"))
    .WithName("Compare")
    .WithTags("Quantities");

qty.MapPost("/convert", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/convert"))
    .WithName("Convert")
    .WithTags("Quantities");

qty.MapPost("/add", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/add"))
    .WithName("Add")
    .WithTags("Quantities");

qty.MapPost("/subtract", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/subtract"))
    .WithName("Subtract")
    .WithTags("Quantities");

qty.MapPost("/divide", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/divide"))
    .WithName("Divide")
    .WithTags("Quantities");

qty.MapGet("/history/all", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/history/all"))
    .WithName("HistoryAll")
    .WithTags("History");

qty.MapGet("/history/operation/{operationType}", (HttpContext ctx, IHttpClientFactory f, string operationType) =>
    ForwardAsync(ctx, f, "qma-service", $"/api/v1/quantities/history/operation/{operationType}"))
    .WithName("HistoryByOperation")
    .WithTags("History");

qty.MapGet("/history/category/{category}", (HttpContext ctx, IHttpClientFactory f, string category) =>
    ForwardAsync(ctx, f, "qma-service", $"/api/v1/quantities/history/category/{category}"))
    .WithName("HistoryByCategory")
    .WithTags("History");

qty.MapGet("/history/errors", (HttpContext ctx, IHttpClientFactory f) =>
    ForwardAsync(ctx, f, "qma-service", "/api/v1/quantities/history/errors"))
    .WithName("HistoryErrors")
    .WithTags("History");

qty.MapGet("/count/{operationType}", (HttpContext ctx, IHttpClientFactory f, string operationType) =>
    ForwardAsync(ctx, f, "qma-service", $"/api/v1/quantities/count/{operationType}"))
    .WithName("OperationCount")
    .WithTags("History");

// ── Health check ───────────────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new
{
    service = "api-gateway",
    status = "healthy",
    routes = new
    {
        authService = authServiceUrl,
        qmaService = qmaServiceUrl
    }
}));

app.Run();