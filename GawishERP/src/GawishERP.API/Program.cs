using System.Diagnostics;
using System.Text;
using GawishERP.API.Authorization;
using GawishERP.API.Middleware;
using GawishERP.Application;
using GawishERP.Application.Common.Settings;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure;
using GawishERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// Controllers
// ===========================================

builder.Services.AddControllers();

// ===========================================
// JWT Settings
// ===========================================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

// ===========================================
// Authentication
// ===========================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("================================");
                Console.WriteLine("JWT Authentication Failed");
                Console.WriteLine(context.Exception.ToString());
                Console.WriteLine("================================");

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Debug.WriteLine("JWT Token Validated");
                return Task.CompletedTask;
            }
        };
    });

// ===========================================
// Authorization
// ===========================================

builder.Services.AddAuthorization();

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionAuthorizationPolicyProvider>();

builder.Services.AddSingleton<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();

// ===========================================
// Swagger
// ===========================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "GawishERP API",
            Version = "v1"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

// ===========================================
// Application
// ===========================================

builder.Services.AddApplication();

// ===========================================
// Infrastructure
// ===========================================

builder.Services.AddInfrastructure(
    builder.Configuration);


// ===========================================
// Build
// ===========================================

var app = builder.Build();

// ===========================================
// Exception Middleware
// ===========================================

app.UseMiddleware<ExceptionMiddleware>();

// ===========================================
// Swagger
// ===========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ===========================================
// Authentication
// ===========================================

app.UseAuthentication();

// ===========================================
// Authorization
// ===========================================

app.UseAuthorization();

// ===========================================
// Seed Database
// ===========================================

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await ApplicationDbContextSeeder.SeedAsync(context);
}

// ===========================================
// Endpoints
// ===========================================

app.MapControllers();

app.Run();