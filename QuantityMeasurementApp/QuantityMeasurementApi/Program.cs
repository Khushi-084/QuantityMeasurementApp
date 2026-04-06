using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuantityMeasurementApi.Middleware;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Service;
using QuantityMeasurementRepository;
using QuantityMeasurementRepository.Database;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementRepository.Services;
using StackExchange.Redis;
using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ════════════════════════════════════════════════════════════════════════════
// 1. CONTROLLERS + JSON
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ════════════════════════════════════════════════════════════════════════════
// 2. DATABASE — EF Core (PostgreSQL if configured, else In-Memory)
// ════════════════════════════════════════════════════════════════════════════
var connectionString = config.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(connectionString))
{
   builder.Services.AddDbContext<QuantityMeasurementDbContext>(opts =>
    opts.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.CommandTimeout(30);
        npgsql.EnableRetryOnFailure(3);
        npgsql.MigrationsAssembly("QuantityMeasurementRepository");
    }));
}
else
{
    builder.Services.AddDbContext<QuantityMeasurementDbContext>(opts =>
        opts.UseInMemoryDatabase("QuantityMeasurementDb"));
}

// ════════════════════════════════════════════════════════════════════════════
// 3. REDIS (optional — skipped if not configured)
// ════════════════════════════════════════════════════════════════════════════
var redisConn = config.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConn))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConn));
    }
    catch { /* Redis is optional — continue without it */ }
}

// ════════════════════════════════════════════════════════════════════════════
// 4. REPOSITORIES
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IQuantityMeasurementApiRepository, QuantityMeasurementApiRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementDatabaseRepository>();

// ════════════════════════════════════════════════════════════════════════════
// 5. BUSINESS LAYER
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<IQuantityMeasurementApiService, QuantityMeasurementApiServiceImpl>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

// ════════════════════════════════════════════════════════════════════════════
// 6. JWT AUTHENTICATION
// ════════════════════════════════════════════════════════════════════════════
var jwtKey    = config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key is required.");
var jwtIssuer = config["Jwt:Issuer"]   ?? "QuantityMeasurementApi";
var jwtAud    = config["Jwt:Audience"] ?? "QuantityMeasurementApi";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer           = true,
        ValidIssuer              = jwtIssuer,
        ValidateAudience         = true,
        ValidAudience            = jwtAud,
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ════════════════════════════════════════════════════════════════════════════
// 7. CORS
// Reads allowed origins from environment variable ALLOWED_ORIGINS
// (comma-separated). Falls back to the Render frontend URL.
// For local dev, localhost:4200 is always included.
// ════════════════════════════════════════════════════════════════════════════
var allowedOriginsEnv = config["ALLOWED_ORIGINS"] ?? "";
var allowedOrigins = allowedOriginsEnv
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .ToList();

// Always include the deployed frontend and local dev
allowedOrigins.Add("https://measure-mate-frontend.onrender.com");
allowedOrigins.Add("http://localhost:4200");

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins.Distinct().ToArray())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ════════════════════════════════════════════════════════════════════════════
// 8. SWAGGER
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "Quantity Measurement API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter JWT Token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// ════════════════════════════════════════════════════════════════════════════
// BUILD APP
// ════════════════════════════════════════════════════════════════════════════
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuantityMeasurementDbContext>();
    try
    {
        if (db.Database.IsRelational())
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database migration/creation failed — using in-memory fallback.");
    }
}

// ════════════════════════════════════════════════════════════════════════════
// MIDDLEWARE PIPELINE
// ════════════════════════════════════════════════════════════════════════════
app.UseGlobalExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
    c.RoutePrefix = "swagger";
});

// IMPORTANT: CORS must come FIRST
app.UseCors("FrontendPolicy");

app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();