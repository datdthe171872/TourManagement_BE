using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using Hangfire;
using Hangfire.SqlServer;
using System.Text;
using TourManagement_BE.BackgroundServices;
using System.Linq;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Helper;
using TourManagement_BE.Helper.Common;
using TourManagement_BE.Mapping;
using TourManagement_BE.Repository.Imple;
using TourManagement_BE.Repository.Interface;
using TourManagement_BE.Service;
using TourManagement_BE.Service.AccountManagement;
using TourManagement_BE.Service.ContractManage;
using TourManagement_BE.Service.PaymentHistory;
using TourManagement_BE.Service.Profile;
using TourManagement_BE.Service.PurchasedServicePackageService;
using TourManagement_BE.Service.ServicePackageService;
using TourManagement_BE.Service.TourManagement;
using TourManagement_BE.Helper;
using Microsoft.Extensions.Options;
using TourManagement_BE.BackgroundServices;
using TourManagement_BE.Helper.Common;
using TourManagement_BE.Helper.Constant;

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

                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.CreateDepartureDateRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<Helper.Validator.UpdateDepartureDateRequestValidator>();
            });


            // Add Hangfire services
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));
            builder.Services.AddHangfireServer();
            // Add the Hangfire server
            builder.Services.AddScoped<ServicePackageResetService>();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register repositories and services
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            //new checkslot
            builder.Services.AddScoped<ISlotCheckService, SlotCheckService>();
            builder.Services.AddScoped<ITourOperatorService, TourOperatorService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IBookingService, BookingService>();


            //builder.Services.AddScoped<IExtraChargeService, ExtraChargeService>();
            builder.Services.AddScoped<IGuideNoteService, GuideNoteService>();
            builder.Services.AddScoped<ITourAcceptanceReportService, TourAcceptanceReportService>();
            builder.Services.AddScoped<IDashboardCustomerService, DashboardCustomerService>();
            builder.Services.AddScoped<IDashboardOperatorService, DashboardOperatorService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IDepartureDateService, DepartureDateService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITourComparisonService, TourComparisonService>();
            builder.Services.AddScoped<IChatbotService, ChatbotService>();
            builder.Services.AddSingleton<IGeminiClient, GeminiClient>();


            //ServicePacakge
            builder.Services.AddScoped<IServicePackageService, ServicePackageService>();
            builder.Services.AddScoped<IPurchasedServicePackageService, PurchasedServicePackageService>();


            //Tour
            builder.Services.AddScoped<ITourService, TourService>();

            //Payment
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddHostedService<EmailPaymentBackgroundService>();
            // Auto cancel overdue bookings & send reminders
            builder.Services.AddHostedService<BookingBackgroundService>();

            //Profile
            builder.Services.AddScoped<IProfileService, ProfileService>();

            //AccountManage
            builder.Services.AddScoped<IAccountService, AccountService>();

            //ContractMange
            builder.Services.AddScoped<IContractService, ContractService>();

            // Register JwtHelper with configuration
            builder.Services.AddScoped<JwtHelper>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new JwtHelper(
                    configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing"),
                    configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing"),
                    configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is missing")
                );
            });

            // Register EmailHelper
            builder.Services.AddScoped<EmailHelper>();

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

            app.UseHangfireDashboard("/hangfire");

            using (var scope = app.Services.CreateScope())
            {
                var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                jobManager.AddOrUpdate<ServicePackageResetService>(
                    "reset-monthly-tour-count",
                    service => service.ResetMonthlyTourCountAsync(),
                    Cron.Monthly(1)
                );
            }

            app.MapControllers();

            // Initialize default admin account
            InitializeDefaultAdmin(app.Services);

            app.Run();
        }

        private static void InitializeDefaultAdmin(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MyDBContext>();

            try
            {
                var roles = context.Roles.ToList();
                if(!roles.Any())
                {
                    var adminRole = new Data.Models.Role
                    {
                        RoleName = Roles.Admin,
                        IsActive = true
                    };
                    context.Roles.Add(adminRole);

                    var customerRole = new Data.Models.Role
                    {
                        RoleName = Roles.Customer,
                        IsActive = true
                    };
                    context.Roles.Add(customerRole);

                    var tourGuideRole = new Data.Models.Role
                    {
                        RoleName = Roles.TourGuide,
                        IsActive = true
                    };
                    context.Roles.Add(tourGuideRole);

                    var opeRole = new Data.Models.Role
                    {
                        RoleName = Roles.TourOperator,
                        IsActive = true
                    };
                    context.Roles.Add(opeRole);

                    context.SaveChanges();
                }
                // Check if default admin user exists
                var adminUser = context.Users.FirstOrDefault(u => u.UserName == "admin");
                if (adminUser == null)
                {
                    // Create default admin user
                    adminUser = new Data.Models.User
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        Password = PasswordHelper.HashPassword("123456"),
                        Address = "System Admin",
                        PhoneNumber = "0000000000",
                        RoleId = adminRole.RoleId,
                        IsActive = true
                    };
                    context.Users.Add(adminUser);
                    context.SaveChanges();

                    Console.WriteLine("✅ Default admin account created successfully!");
                    Console.WriteLine("Username: admin");
                    Console.WriteLine("Password: 123456");
                    Console.WriteLine("Email: admin@gmail.com");
                }
                else
                {
                    Console.WriteLine("ℹ️  Default admin account already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating default admin account: {ex.Message}");
            }
        }
    }
}
