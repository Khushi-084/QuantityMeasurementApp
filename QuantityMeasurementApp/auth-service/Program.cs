using BusinessService.Auth.Interface;
using BusinessService.Auth.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryService.Auth.Cache;
using RepositoryService.Auth.DBContext;
using RepositoryService.Auth.Interface;
using RepositoryService.Auth.Services;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config  = builder.Configuration;

// ── Controllers + JSON ────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy        = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ── Database ──────────────────────────────────────────────────────────────
var connectionString = config.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
    builder.Services.AddDbContext<AuthDbContext>(opts =>
        opts.UseSqlServer(connectionString, sql =>
        {
            sql.CommandTimeout(30);
            sql.EnableRetryOnFailure(3);
            sql.MigrationsAssembly("AuthService");
        }));
else
    builder.Services.AddDbContext<AuthDbContext>(opts =>
        opts.UseInMemoryDatabase("AuthDb"));

// ── Redis Cache ───────────────────────────────────────────────────────────
var redisConn = config.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConn))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConn));
        builder.Services.AddSingleton<IUserCache, RedisUserCache>();
        Console.WriteLine($"[AUTH] Redis cache enabled: {redisConn}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AUTH] Redis failed ({ex.Message}) — falling back to NullUserCache.");
        builder.Services.AddSingleton<IUserCache, NullUserCache>();
    }
}
else
{
    builder.Services.AddSingleton<IUserCache, NullUserCache>();
    Console.WriteLine("[AUTH] Redis not configured — using NullUserCache.");
}
builder.Services.AddSwaggerGen();

// ── Repository ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ── Business Layer ────────────────────────────────────────────────────────
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();

// ── JWT Auth ──────────────────────────────────────────────────────────────
var jwtKey    = config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key is required.");
var jwtIssuer = config["Jwt:Issuer"]   ?? "QuantityMeasurementApi";
var jwtAud    = config["Jwt:Audience"] ?? "QuantityMeasurementApi";

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer           = true, ValidIssuer   = jwtIssuer,
        ValidateAudience         = true, ValidAudience = jwtAud,
        ValidateLifetime         = true, ClockSkew     = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddPolicy("InternalPolicy", p =>
        p.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

// ── OpenAPI (replaces Swashbuckle) ─────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Auth Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Migrate / create DB ───────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    try
    {
        if (db.Database.IsRelational()) db.Database.Migrate();
        else db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        log.LogWarning(ex, "DB migration failed — using in-memory fallback.");
    }
}
app.UseSwagger();
app.UseSwaggerUI();
// ── Pipeline ──────────────────────────────────────────────────────────────
// ── Scalar UI at /scalar/v1 (replaces Swagger UI) ─────────────────────────

app.UseCors("InternalPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { service = "auth-service", status = "healthy" }));

app.MapGet("/health/cache", async ([FromServices] IConnectionMultiplexer? redis) =>
{
    if (redis is null)
        return Results.Ok(new { service = "auth-service", cache = "disabled", status = "healthy" });
    try
    {
        await redis.GetDatabase().PingAsync();
        return Results.Ok(new { service = "auth-service", cache = "redis", status = "healthy" });
    }
    catch (Exception ex)
    {
        return Results.Json(
            new { service = "auth-service", cache = "redis", status = "unhealthy", error = ex.Message },
            statusCode: 503);
    }
});

app.Run();
