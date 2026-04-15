using BusinessService.Qma.Interface;
using BusinessService.Qma.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using RepositoryService.Qma.DBContext;
using RepositoryService.Qma.Interface;
using RepositoryService.Qma.Services;
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
    builder.Services.AddDbContext<QmaDbContext>(opts =>
        opts.UseSqlServer(connectionString, sql =>
        {
            sql.CommandTimeout(30);
            sql.EnableRetryOnFailure(3);
            sql.MigrationsAssembly("QmaService");
        }));
else
    builder.Services.AddDbContext<QmaDbContext>(opts =>
        opts.UseInMemoryDatabase("QmaDb"));

// ── Redis Cache ───────────────────────────────────────────────────────────
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
        Console.WriteLine($"[QMA] Redis failed ({ex.Message}) — measurement cache disabled.");
    }
}

// ── Repository ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IQmaRepository, QmaRepository>();

// ── Business Layer ────────────────────────────────────────────────────────
builder.Services.AddScoped<IQmaService, QmaServiceImpl>();

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
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title   = "QMA Service";
        document.Info.Version = "v1";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// ── Migrate / create DB ───────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QmaDbContext>();
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

// ── Pipeline ──────────────────────────────────────────────────────────────
// ── Scalar UI at /scalar/v1 (replaces Swagger UI) ─────────────────────────
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "QMA Service";
});
app.UseCors("InternalPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { service = "qma-service", status = "healthy" }));

app.Run();
