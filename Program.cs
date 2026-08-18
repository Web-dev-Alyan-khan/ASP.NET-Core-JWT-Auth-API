using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyWebApi.Data;
using MyWebApi.Helpers;
using MyWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1. Database - EF Core + SQL Server
// ======================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ======================================================
// 2. JWT Authentication
// ======================================================

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

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });

// ======================================================
// 3. Authorization
// ======================================================

builder.Services.AddAuthorization();

// ======================================================
// 4. Services
// ======================================================

builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ======================================================
// 5. Controllers
// ======================================================

builder.Services.AddControllers();

// ======================================================
// 6. Swagger
// ======================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyWebApi",
        Version = "v1",
        Description = "ASP.NET Core Web API with JWT Authentication"
    });

    // JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",

        In = ParameterLocation.Header,

        Description =
            "Enter your JWT token like this: Bearer {your-token}"
    });

    // Apply JWT authentication to endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ======================================================
// 7. Swagger UI
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ======================================================
// 8. Middleware
// ======================================================

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// ======================================================
// 9. Controllers
// ======================================================

app.MapControllers();

app.Run();