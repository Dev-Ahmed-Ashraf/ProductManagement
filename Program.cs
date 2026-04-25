using DBS_Task.Application.Auth.Commands.Login;
using DBS_Task.Application.Common.Behaviors;
using DBS_Task.Application.Common.Constants;
using DBS_Task.Application.Common.Interfaces;
using DBS_Task.Domain.Entities;
using DBS_Task.Infrastructure.Data.DBContext;
using DBS_Task.Infrastructure.Data.Interceptors;
using DBS_Task.Infrastructure.Data.Seeders;
using DBS_Task.Infrastructure.Security;
using DBS_Task.Infrastructure.Services;
using FluentValidation;
using Hotel_Booking_API.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
//  Core Services
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();


// ============================================================
//  Swagger
// ============================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "DBS Internship Project – Product Management System"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});


// ============================================================
//  Database  (with AuditInterceptor)
// ============================================================
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditInterceptor>();
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(interceptor);
});


// ============================================================
//  MediatR
// ============================================================
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssemblyContaining<LoginUserCommand>();
});


// ============================================================
//  AutoMapper  ← FIX: scan BOTH assemblies
// ============================================================
builder.Services.AddAutoMapper(
    typeof(Program).Assembly,
    typeof(LoginUserCommand).Assembly
);


// ============================================================
//  FluentValidation + Pipeline Behavior
// ============================================================
builder.Services.AddValidatorsFromAssemblyContaining<LoginUserCommand>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


// ============================================================
//  ASP.NET Core Identity
// ============================================================
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// ============================================================
//  JWT Settings  (Options pattern with startup validation)
// ============================================================
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
.AddOptions<RefreshTokenSettings>()
.Bind(
 builder.Configuration.GetSection(
   "RefreshTokenSettings"))
.ValidateOnStart();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;


// ============================================================
//  Authentication – JWT Bearer
// ============================================================
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

            ClockSkew = TimeSpan.Zero
        };

        // Return clean JSON on 401 / 403 instead of empty responses
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "Unauthenticated. Please login and include a valid Bearer token."
                    }));
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "Forbidden. You do not have permission to perform this action."
                    }));
            }
        };
    });


// ============================================================
//  Authorization Policies  (permission-based)
// ============================================================
builder.Services.AddAuthorization(options =>
{
    var policies = new[]
    {
        AppClaims.ProductsView,
        AppClaims.ProductsCreate,
        AppClaims.ProductsDelete,
        AppClaims.ProductsChangeStatus,
        AppClaims.ProductStatusHistoriesView,
        AppClaims.UsersView,
        AppClaims.UsersCreate,
        AppClaims.StatisticsView
    };

    foreach (var permission in policies)
        options.AddPolicy(
            permission,
            policy => policy.RequireClaim("permission", permission));
});


// ============================================================
//  CORS
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});


// ============================================================
//  Application Services
// ============================================================
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IJWTService, JwtService>();


// ============================================================
//  Build
// ============================================================
var app = builder.Build();


// ============================================================
//  Global Exception Handler 
// ============================================================
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var error = context.Features
                .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

            if (error is not null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = error.Error.Message,
                    inner = error.Error.InnerException?.Message,
                    trace = error.Error.StackTrace
                });
            }
        });
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}


// ============================================================
//  Seed Roles & Claims
// ============================================================
using (var scope = app.Services.CreateScope())
{
    await RolesAndClaimsSeeder.SeedAsync(scope.ServiceProvider);
}


// ============================================================
//  Middleware Pipeline  (ORDER MATTERS)
// ============================================================
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();