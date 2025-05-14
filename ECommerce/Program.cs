using ECommerce.ChatServices;
using ECommerce.Core;
using ECommerce.Core.RepoInterface;
using ECommerce.Core.Services;
using ECommerce.Errors;
using ECommerce.Helper;
using ECommerce.Repo;
using ECommerce.Repo.Data;
using ECommerce.Repo.GraphQL;
using ECommerce.Repo.GraphQL.Mutations;
using ECommerce.Repo.GraphQL.Queries;
using ECommerce.Service;
using ECommerce.Service.Emails;
using GraphiQl;
using GraphQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PdfSharp.Charting;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ECommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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
            #endregion

            //Gemini
            builder.Services.AddHttpClient<GeminiService>(c => c.Timeout = TimeSpan.FromSeconds(300));

            //Controller
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>))
                            .AddScoped<IUnitWork, UnitWork>()
                            .AddScoped<QLSchema>()
                            .AddScoped<ProductQuery>()
                            .AddScoped<ProductMutation>()

                            //Redis Conn
                            .AddSingleton<IConnectionMultiplexer>(options =>
                            {
                                var conn = builder.Configuration.GetConnectionString("RedisConnection");
                                if (string.IsNullOrEmpty(conn))
                                {
                                    throw new InvalidOperationException("Redis connection string is not configured.");
                                }
                                return ConnectionMultiplexer.Connect(conn);
                            })
                            //.AddSingleton<Ngrok>()
                            .AddSingleton<ICache, CacheService>()
                            .AddSingleton<ProductPicture>()
                            .AddSingleton<PictureGlb>()
                            .AddScoped<ProductisFavorite>()
                            .AddScoped<ProductisLike>()
                            .AddSingleton(provider =>
                            {
                                var supabaseUrl = builder.Configuration["SupaBaseClient:Url"];
                                var supabaseKey = builder.Configuration["SupaBaseClient:Key"];
                                if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
                                {
                                    throw new InvalidOperationException("Supabase configuration is missing in appsettings.json.");
                                }
                                return new Supabase.Client(supabaseUrl, supabaseKey);
                            })

                            .AddTransient<IMailServices, MailServices>()
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
            })
                            .Configure<MailSettings>(builder.Configuration.GetSection("MaillSettings"));
            //.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));

            builder.Services.AddAuthentication("Sanctum")
                             .AddScheme<AuthenticationSchemeOptions, SanctumAuthenticationHandler>("Sanctum", options => { });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Sanctum", policy =>
                    policy.RequireAuthenticatedUser());

                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("admin", "Admin"));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", a =>
                {
                    a.SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //GraphQL
            builder.Services.AddGraphQL(builder => builder
                .AddSystemTextJson()
            );

            builder.Services.AddSignalR();
            builder.Services.AddResponseCompression(opt =>
                opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" })
            );
            builder.Services.AddHttpClient<GeminiService>();
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

            //GraphQL
            app.UseGraphQL<QLSchema>();
            app.UseGraphiQl("/ui/graphql");

            app.UseHttpsRedirection();

            //PictureUrl
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("MyPolicy");

            // Identity
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");
            #endregion

            #region Ngrok
            var _ngrokProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Ngrok\ngrok.exe",
                    Arguments = $"http --url=rational-deep-dinosaur.ngrok-free.app https://localhost:7182",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                Console.WriteLine("Starting ngrok process...");
                _ngrokProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start ngrok process: {ex.Message}");
                _ngrokProcess.Kill();
                _ngrokProcess.Dispose();
                Console.WriteLine("Ngrok process terminated.");
            }
            #endregion

            app.Run();
        }
    }
}
