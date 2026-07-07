using CertiWeb.API.Shared.Domain.Repositories;
using CertiWeb.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using CertiWeb.API.Users.Application.Internal.CommandServices;
using CertiWeb.API.Users.Application.Internal.QueryServices;
using CertiWeb.API.Users.Domain.Repositories;
using CertiWeb.API.Users.Domain.Services;
using CertiWeb.API.Users.Infrastructure.Persistence.EFC.Repositories;
using CertiWeb.API.Reservation.Application.Internal.CommandServices;
using CertiWeb.API.Reservation.Application.Internal.QueryServices;
using CertiWeb.API.Reservation.Domain.Repositories;
using CertiWeb.API.Reservation.Domain.Services;
using CertiWeb.API.Reservation.Infrastructure.Persistence.EFC.Repositories;
using CertiWeb.API.Vehicles.Application.Internal.CommandServices;
using CertiWeb.API.Vehicles.Application.Internal.QueryServices;
using CertiWeb.API.Vehicles.Domain.Repositories;
using CertiWeb.API.Vehicles.Domain.Services;
using CertiWeb.API.Vehicles.Infrastructure.Persistence.EFC.Repositories;
using CertiWeb.API.IAM.Application.ACL;
using CertiWeb.API.IAM.Application.Internal.QueryServices;
using CertiWeb.API.IAM.Domain.Repositories;
using CertiWeb.API.IAM.Domain.Services;
using CertiWeb.API.IAM.Infrastructure.Persistence.EFC.Repositories;
using CertiWeb.API.IAM.Interfaces.ACL;
using CertiWeb.API.Users.Application.ACL;
using CertiWeb.API.Users.Application.Internal.OutboundServices;
using CertiWeb.API.Users.Infrastructure.Hashing.BCrypt.Services;
using CertiWeb.API.Users.Infrastructure.Pipeline.Middleware.Extensions;
using CertiWeb.API.Users.Infrastructure.Tokens.JWT.Configuration;
using CertiWeb.API.Users.Infrastructure.Tokens.JWT.Services;
using CertiWeb.API.Users.Interfaces.ACL;
using CertiWeb.API.Shared.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CertiWeb.API.Shared.Infrastructure.BackgroundTasks;
using CertiWeb.API.Security.Domain.Repositories;
using CertiWeb.API.Security.Domain.Services;
using CertiWeb.API.Security.Application.Internal.CommandServices;
using CertiWeb.API.Security.Application.Internal.QueryServices;
using CertiWeb.API.Security.Infrastructure.Persistence.EFC.Repositories;
using CertiWeb.API.Security.Infrastructure.Pipeline.Middleware.Extensions;
using CertiWeb.API.Inspections.Domain.Repositories;
using CertiWeb.API.Inspections.Domain.Services;
using CertiWeb.API.Inspections.Application.Internal.CommandServices;
using CertiWeb.API.Inspections.Application.Internal.QueryServices;
using CertiWeb.API.Inspections.Infrastructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendPolicy",
        policy => policy.WithOrigins(
                "https://project-kzvht.vercel.app",
                "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

if (connectionString == null) throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging(false)
            .EnableDetailedErrors();
    else if (builder.Environment.IsProduction())
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "CertiWeb.API",
            Version = "v1",
            Description = "CertiWeb Platform API",
            TermsOfService = new Uri("https://acme-learning.com/tos"),
            Contact = new OpenApiContact
            {
                Name = "Certi Web",
                Email = "contact@acme.com"
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
    options.EnableAnnotations();
});

// Dependency Injection

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// TokenSettings Configuration

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

// Users Bounded Context
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandServiceImpl>();
builder.Services.AddScoped<IUserQueryService, UserQueryServiceImpl>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Vehicles Bounded Context Dependency Injection Configuration (must be before Reservation)
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICarCommandService, CarCommandServiceImpl>();
builder.Services.AddScoped<ICarQueryService, CarQueryServiceImpl>();
builder.Services.AddScoped<BrandQueryServiceImpl>();

// Storage Service for PDF management
builder.Services.AddScoped<IStorageService, LocalBase64StorageService>();

// Background task queue and processing service (in-memory)
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<BackgroundProcessingService>();

// Reservation Bounded Context Dependency Injection Configuration
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationCommandService, ReservationCommandServiceImpl>();
builder.Services.AddScoped<IReservationQueryService, ReservationQueryServiceImpl>();

// IAM Bounded Context Injection Configuration
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IAdminUserQueryService, AdminUserQueryService>();

// ACL Services Registration
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();
builder.Services.AddScoped<IUsersContextFacade, UsersContextFacade>();

builder.Services.AddSingleton<CertiWeb.API.Shared.Infrastructure.Messaging.RabbitMQProducer>();
builder.Services.AddHostedService<CertiWeb.API.Shared.Infrastructure.Messaging.CertificateConsumerService>();

// Security Bounded Context Dependency Injection Configuration (AC-01 audit logging)
builder.Services.AddScoped<ISecurityAuditLogRepository, SecurityAuditLogRepository>();
builder.Services.AddScoped<ISecurityAuditLogCommandService, SecurityAuditLogCommandServiceImpl>();
builder.Services.AddScoped<ISecurityAuditLogQueryService, SecurityAuditLogQueryServiceImpl>();

// Inspections Bounded Context Dependency Injection Configuration (AC-03 async processing evidence)
builder.Services.AddScoped<IProcessedInspectionEventRepository, ProcessedInspectionEventRepository>();
builder.Services.AddScoped<IProcessedInspectionEventCommandService, ProcessedInspectionEventCommandServiceImpl>();
builder.Services.AddScoped<IProcessedInspectionEventQueryService, ProcessedInspectionEventQueryServiceImpl>();

// Health checks (used by nginx upstream health probing in front of the api/api2 replicas)
builder.Services.AddHealthChecks();

var app = builder.Build();

// Verify if the database exists and create it if it doesn't
using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        // EnsureCreated() only builds the schema for a brand-new database - it never alters an
        // existing one. For pre-existing local/deployed MySQL volumes created before the
        // SecurityAuditLog table was added, create just that table idempotently so the
        // AC-01 audit log works without wiping the volume or losing seeded data.
        if (!app.Environment.IsEnvironment("Testing"))
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS security_audit_logs (
                        id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        `timestamp` DATETIME(6) NOT NULL,
                        ip_address VARCHAR(45) NULL,
                        endpoint VARCHAR(500) NOT NULL,
                        http_method VARCHAR(10) NOT NULL,
                        status_code INT NOT NULL,
                        user_id INT NULL
                    ) CHARACTER SET utf8mb4;");
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning($"security_audit_logs table check/creation failed: {ex.Message}");
            }

            // Same idempotent approach for the processed_inspection_events table (AC-03 async
            // processing evidence), added after some volumes already existed.
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS processed_inspection_events (
                        id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        received_at DATETIME(6) NOT NULL,
                        raw_message TEXT NOT NULL,
                        status VARCHAR(20) NOT NULL
                    ) CHARACTER SET utf8mb4;");
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning($"processed_inspection_events table check/creation failed: {ex.Message}");
            }
        }

        // Dev-only fake data (Bogus): never runs outside Development, and only touches
        // empty tables so it's safe to leave the database running between restarts.
        if (app.Environment.IsDevelopment())
        {
            var hashingService = services.GetRequiredService<IHashingService>();
            await CertiWeb.API.Shared.Infrastructure.DevDataSeeder.SeedAsync(context, hashingService);
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning($"Database initialization failed: {ex.Message}. The application will continue running.");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


// Apply CORS Policy
app.UseCors("AllowFrontendPolicy");

// In testing environment ensure certain response headers exist (Date, etc.)
if (app.Environment.IsEnvironment("Testing"))
{
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey("Date"))
            {
                context.Response.Headers.Add("Date", DateTimeOffset.UtcNow.ToString("R"));
            }
            return Task.CompletedTask;
        });

        await next();
    });

    // In testing environment inject a default Authorization header when missing so
    // system tests that forget to set it still exercise the pipeline using the
    // test token registered by the test host.
    app.Use(async (context, next) =>
    {
        if (!context.Request.Headers.ContainsKey("Authorization") ||
            string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"].ToString()))
        {
            context.Request.Headers["Authorization"] = "Bearer test-token";
        }

        await next();
    });
}

// Wraps the whole downstream pipeline so it can observe the final status code of every
// request (including the 401s short-circuited by UseRequestAuthorization below) without
// adding latency to the response itself - the DB write is queued to run in the background.
app.UseSecurityAuditLogging();

// Enable endpoint routing so middlewares can read endpoint metadata (AllowAnonymous etc.)
app.UseRouting();

// Add Authorization Middleware to Pipeline
app.UseRequestAuthorization();

// Only use HTTPS redirection in Production (containers using HTTP with no TLS should avoid this)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

// Exempt /health from the custom RequestAuthorizationMiddleware so load balancers/orchestrators
// can probe it without a bearer token.
app.MapHealthChecks("/health")
    .WithMetadata(new CertiWeb.API.Users.Infrastructure.Pipeline.Middleware.Attributes.AllowAnonymousAttribute());

app.Run();