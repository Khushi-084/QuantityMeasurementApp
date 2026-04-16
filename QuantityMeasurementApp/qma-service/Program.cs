using BusinessService.Qma.Interface;
using BusinessService.Qma.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryService.Qma.DBContext;
using RepositoryService.Qma.Interface;
using RepositoryService.Qma.Services;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ─────────────────────────────────────────────
// Controllers + JSON
// ─────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ─────────────────────────────────────────────
// Database
// ─────────────────────────────────────────────
var connectionString = config.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<QmaDbContext>(opts =>
        opts.UseSqlServer(connectionString, sql =>
        {
            sql.CommandTimeout(30);
            sql.EnableRetryOnFailure(3);
            sql.MigrationsAssembly("QmaService");
        }));
}
else
{
    builder.Services.AddDbContext<QmaDbContext>(opts =>
        opts.UseInMemoryDatabase("QmaDb"));
}

// ─────────────────────────────────────────────
// Redis
// ─────────────────────────────────────────────
var redisConn = config.GetConnectionString("Redis");

if (!string.IsNullOrWhiteSpace(redisConn))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConn));

        Console.WriteLine($"[QMA] Redis cache enabled: {redisConn}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[QMA] Redis failed ({ex.Message}) — cache disabled");
    }
}

// ─────────────────────────────────────────────
// DI
// ─────────────────────────────────────────────
builder.Services.AddScoped<IQmaRepository, QmaRepository>();
builder.Services.AddScoped<IQmaService, QmaServiceImpl>();

// ─────────────────────────────────────────────
// JWT Auth
// ─────────────────────────────────────────────
var jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
var jwtIssuer = config["Jwt:Issuer"] ?? "QuantityMeasurementApi";
var jwtAudience = config["Jwt:Audience"] ?? "QuantityMeasurementApi";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────
// CORS
// ─────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("InternalPolicy", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ─────────────────────────────────────────────
// Swagger
// ─────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QMA Service",
        Version = "v1",
        Description = "Quantity Measurement API Service"
    });

    // JWT support in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ─────────────────────────────────────────────
// DB Init
// ─────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QmaDbContext>();

    try
    {
        if (db.Database.IsRelational())
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        log.LogWarning(ex, "DB migration failed — using fallback");
    }
}

// ─────────────────────────────────────────────
// Middleware
// ─────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QMA Service v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("InternalPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ─────────────────────────────────────────────
// Health Check
// ─────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new
{
    service = "qma-service",
    status = "healthy"
}));

app.Run();