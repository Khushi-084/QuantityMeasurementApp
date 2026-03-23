using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuantityMeasurementApi.Middleware;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Service;
using QuantityMeasurementRepository.Database;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementRepository.Services;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config  = builder.Configuration;

// ════════════════════════════════════════════════════════════════════════════
// 1. CONTROLLERS + JSON
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy   = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ════════════════════════════════════════════════════════════════════════════
// 2. DATABASE — EF Core + SQL Server (falls back to InMemory if no conn string)
// ════════════════════════════════════════════════════════════════════════════
var connectionString = config.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(opts =>
        opts.UseSqlServer(connectionString, sql =>
        {
            sql.CommandTimeout(30);
            sql.EnableRetryOnFailure(3);
            // Tell EF Core where to find migrations
            sql.MigrationsAssembly("QuantityMeasurementRepository");
        }));
}
else
{
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(opts =>
        opts.UseInMemoryDatabase("QuantityMeasurementDb"));
}

// ════════════════════════════════════════════════════════════════════════════
// 3. REDIS — optional, silent fallback if not configured
// ════════════════════════════════════════════════════════════════════════════
var redisConn = config.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConn))
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisConn));

// ════════════════════════════════════════════════════════════════════════════
// 4. REPOSITORY LAYER — Scoped (one instance per request)
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IQuantityMeasurementApiRepository, QuantityMeasurementApiRepository>();
builder.Services.AddScoped<IUserRepository,                   UserRepository>();

// UC16 IQuantityMeasurementRepository — required by QuantityMeasurementServiceImpl.
// Uses the in-memory CacheRepository (Singleton) so the UC16 service can save records.
builder.Services.AddSingleton<QuantityMeasurementRepository.IQuantityMeasurementRepository>(
    _ => QuantityMeasurementRepository.QuantityMeasurementCacheRepository.Instance);

// ════════════════════════════════════════════════════════════════════════════
// 5. BUSINESS LAYER
//    UC16 service (sync) wired as Scoped — used internally by UC17 async wrapper.
//    UC17 services Scoped; JWT and Encryption Singleton (stateless).
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IQuantityMeasurementService,    QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<IQuantityMeasurementApiService, QuantityMeasurementApiServiceImpl>();
builder.Services.AddScoped<IUserService,                   UserService>();
builder.Services.AddSingleton<IJwtService,                 JwtService>();
builder.Services.AddSingleton<IEncryptionService,          EncryptionService>();

// ════════════════════════════════════════════════════════════════════════════
// 6. JWT AUTHENTICATION
// ════════════════════════════════════════════════════════════════════════════
var jwtKey    = config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key is required.");
var jwtIssuer = config["Jwt:Issuer"]   ?? "QuantityMeasurementApi";
var jwtAud    = config["Jwt:Audience"] ?? "QuantityMeasurementApi";

builder.Services
    .AddAuthentication(opts =>
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
        opts.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                if (ctx.Exception is SecurityTokenExpiredException)
                    ctx.Response.Headers.Append("Token-Expired", "true");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ════════════════════════════════════════════════════════════════════════════
// 7. SWAGGER / OPENAPI with JWT Bearer button
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Quantity Measurement API",
        Version     = "v1",
        Description = "UC17 Web API — Compare, Convert, Add, Subtract, Divide quantities.\n" +
                      "Secured with JWT. Flow: POST /signup → copy token → Authorize → use endpoints."
    });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header,
        Description = "Paste JWT token here."
    };
    opts.AddSecurityDefinition("Bearer", scheme);
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference
            { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
    var xmlPath = Path.Combine(AppContext.BaseDirectory,
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xmlPath)) opts.IncludeXmlComments(xmlPath);
});

// ════════════════════════════════════════════════════════════════════════════
// 8. CORS + HEALTH CHECKS
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddCors(opts =>
    opts.AddPolicy("DevCors", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<QuantityMeasurementDbContext>("database");

// ════════════════════════════════════════════════════════════════════════════
// BUILD + PIPELINE
// ════════════════════════════════════════════════════════════════════════════
var app = builder.Build();

// ── Apply EF Core migrations automatically on startup ─────────────────────
// Migrate() creates the database + tables if they don't exist, AND applies
// any pending migrations. Safe to call on every startup.
// Unlike EnsureCreated(), it works even when the database already exists.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuantityMeasurementDbContext>();
    if (db.Database.IsSqlServer())
        db.Database.Migrate();      // SQL Server → apply migrations
    else
        db.Database.EnsureCreated(); // InMemory → just create schema
}

app.UseGlobalExceptionHandling();

// Enable Swagger in ALL environments
app.UseSwagger();
app.UseSwaggerUI(opts =>
{
    opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
    opts.RoutePrefix = "swagger";
    opts.DisplayRequestDuration();
});

// Skip HTTPS redirect (running on plain HTTP locally)
// app.UseHttpsRedirection();
app.UseCors("DevCors");
app.UseMiddleware<QuantityMeasurementApi.Middleware.JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();