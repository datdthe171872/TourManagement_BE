using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using TourManagement_BE.Data;
using TourManagement_BE.Mapping;
using TourManagement_BE.Repository.Imple;
using TourManagement_BE.Repository.Interface;
using TourManagement_BE.Service;
using TourManagement_BE.Helper;
using Microsoft.Extensions.Options;
using TourManagement_BE.Models;

namespace TourManagement_BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MyDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            builder.Services.AddControllers().AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.LoginRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.RegisterRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.ForgotPasswordRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.CreateTourOperatorRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.UpdateTourOperatorRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.CreateBookingRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.UpdateBookingRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.CreateExtraChargeRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.UpdateExtraChargeRequestValidator>();
            });
           
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register repositories and services
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            //new checkslot
            builder.Services.AddScoped<ISlotCheckService, SlotCheckService>();
            builder.Services.AddHostedService<ExpiredPackageCleanupService>();
            builder.Services.AddScoped<ITourOperatorService, TourOperatorService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IExtraChargeService, ExtraChargeService>();
            builder.Services.AddScoped<IGuideNoteService, GuideNoteService>();
            builder.Services.AddScoped<IDashboardCustomerService, DashboardCustomerService>();
            builder.Services.AddScoped<IDashboardOperatorService, DashboardOperatorService>();

            // Configure JWT Authentication
            var secretKey = builder.Configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is missing in configuration.");
            }

            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaseAPI", Version = "v1" });
                // Enable JWT in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {your token}' to authenticate."
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
            new string[] {}
        }
    });
            });
            // Configure AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(typeof(Program));


            // Bind CloudinarySettings
            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));

            // Register Cloudinary client as singleton
            builder.Services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                var account = new CloudinaryDotNet.Account(
                    settings.CloudName,
                    settings.ApiKey,
                    settings.ApiSecret
                );
                return new CloudinaryDotNet.Cloudinary(account);
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
