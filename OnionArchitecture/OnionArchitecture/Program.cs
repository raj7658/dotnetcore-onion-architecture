using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnionArchitecture.Application.Interfaces;
using OnionArchitecture.Application.Services;
using OnionArchitecture.Domain.Interfaces;
using OnionArchitecture.Infrastructure.Context;
using OnionArchitecture.Infrastructure.Repositories;
using OnionArchitecture.Middlewares;
using System;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // Database Connection

        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        //builder.Services.AddDbContext<AppDbContext>(options =>
        //    options.UseSqlServer("DefaultConnection"));

        //Dependency Injection
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // Authentication
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"];

        var key = Encoding.ASCII.GetBytes(jwtKey);
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            // Add a security definition for Bearer token
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' followed by a space and then your token.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            // Add security requirement to apply Bearer token to all operations
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
                    new string[] {}
                }
         });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<LoggerMiddleware>();
        app.UseMiddleware<JwtValidationMiddleware>();

        app.UseAuthentication();
        app.UseRouting();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}