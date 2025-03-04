using ECommerce.Core;
using ECommerce.Core.RepoInterface;
using ECommerce.Core.Repos;
using ECommerce.Core.Services;
using ECommerce.Errors;
using ECommerce.Helper;
using ECommerce.Repo;
using ECommerce.Repo.Data;
using ECommerce.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json.Serialization;
using ECommerce.Controllers;

namespace ECommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region NGrok
            var ngrokProcess = new Process();
            ngrokProcess.EnableRaisingEvents = true;
            ngrokProcess.StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Ngrok\ngrok.exe",
                Arguments = $"http --url=rational-deep-dinosaur.ngrok-free.app https://localhost:7182",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                Console.WriteLine("Starting ngrok process...");
                ngrokProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start ngrok process: {ex.Message}");
            }
            #endregion

            var builder = WebApplication.CreateBuilder(args);

            #region Config Services - Add services to the container.
            builder.Services
                .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
                .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "ECommerce Demo",
                    Version = "v1"
                });

                // Define the security scheme
                options.AddSecurityDefinition("Sanctum", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the Sanctum token.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, // Correct Type for Bearer Token
                    Scheme = "Bearer",             // Ensure this matches the Bearer scheme0
                    BearerFormat = "JWT"           // Optional, for display purposes
                });

                // Add security requirements
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Sanctum"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            #region Connection
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("SupaConnection"))
                    .EnableDetailedErrors();
            });

            //builder.Services.AddDbContext<UserContext>(options =>
            //{
            //    options.UseNpgsql(builder.Configuration.GetConnectionString("EventConnection"))
            //        .EnableDetailedErrors();
            //});

            //Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var conn = builder.Configuration.GetConnectionString("RedisConnection");
                if (string.IsNullOrEmpty(conn))
                {
                    throw new InvalidOperationException("Redis connection string is not configured.");
                }
                return ConnectionMultiplexer.Connect(conn);
            });
            #endregion

            //Controller
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>))
                            .AddScoped<IBasketRepo, BasketRepo>()
                            .AddScoped<IUnitWork, UnitWork>()
                            //.AddScoped<IToken, TokenService>()
                            //.AddScoped<IOrder, OrderService>()
                            //.AddScoped<IPayment, PaymentService>()
                            .AddSingleton<ICache, CacheService>()
                            .AddSingleton<ProductPicture>()
                            .AddSingleton<PictureGlb>()
                            .AddAutoMapper(typeof(MappingProfiles));

            //Validation
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();
                    var result = new ValidationResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(result);
                };
            });

            //Identity
            //builder.Services.AddIdentity<AppUser, IdentityRole>()
            //                .AddEntityFrameworkStores<UserContext>();

            builder.Services.AddAuthentication("Sanctum")
                            .AddScheme<AuthenticationSchemeOptions, SanctumAuthenticationHandler>("Sanctum", null);

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Sanctum", policy =>
                    policy.RequireAuthenticatedUser());
            });

            /*JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true, // Ensure the token's issuer matches the configured issuer
                        ValidIssuer = builder.Configuration["JWT:Issuer"],

                        ValidateAudience = true, // Ensure the token's audience matches the configured audience
                        ValidAudience = builder.Configuration["JWT:Audience"],

                        ValidateLifetime = true, // Ensure the token is not expired
                        ClockSkew = TimeSpan.FromDays(double.Parse(builder.Configuration["JWT:Duration"])),

                        ValidateIssuerSigningKey = true, // Validate the signing key to ensure the token's integrity
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),

                        // Additional options
                        RequireExpirationTime = true
                    };
                }); // Manager / SignIn / Roles
            */

            builder.Services.AddAuthorizationBuilder();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", a =>
                {
                    a.AllowAnyHeader();
                    a.AllowAnyMethod();
                    a.AllowAnyOrigin();
                });
            });
            #endregion
            
            var app = builder.Build();

            #region Update-Database && DataSeed
            var scope = app.Services.CreateScope();
            var Services = scope.ServiceProvider;
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbContext = Services.GetRequiredService<StoreContext>();
                await dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(dbContext);

                //var userManager = Services.GetRequiredService<UserManager<AppUser>>();
                //var identityContext = Services.GetRequiredService<UserContext>();
                //await identityContext.Database.MigrateAsync();
                //await UserContextSeed.SeedUserAsync(userManager);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, $"An Error Occured During The Migration");
            }
            #endregion

            #region Config - Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
;
            app.UseHttpsRedirection();
            
            // PictureUrl
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("MyPolicy");

            // Identity
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
