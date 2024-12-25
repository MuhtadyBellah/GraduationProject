using ECommerce.Core;
using ECommerce.Core.Repos;
using ECommerce.Helper;
using ECommerce.Repo;
using ECommerce.Repo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Diagnostics;

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
            builder.Services.AddControllers();
            //builder.Services
            //    .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
            //    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options=>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "ECommerce Demo",
                    Version = "v1"
                });

                // Define the security scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the JWT token.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, // Correct Type for Bearer Token
                    Scheme = "Bearer",             // Ensure this matches the Bearer scheme
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
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            //Connection
            //NpgSql
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("SupaConnection"))
                    .EnableDetailedErrors();
            });

            //Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var conn = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(conn);
            });

            //Controller
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IUnitWork, UnitWork>();
            //builder.Services.AddScoped<IToken, TokenService>();

            //PicResolver && Identity
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddSingleton<ProductPicture>();

            
            builder.Services.AddAuthorizationBuilder();

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidIssuer = builder.Configuration["JWT:Issuer"],

            //        ValidateAudience = true,
            //        ValidAudience = builder.Configuration["JWT:Audience"],

            //        ValidateLifetime = true,
            //        ClockSkew = TimeSpan.Zero,

            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            //    };
            //});

            #region ErrorHandling
            #endregion

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
                //var identityContext = Services.GetRequiredService<IdentityContext>();
                //await identityContext.Database.MigrateAsync();
                //await IdentityContextSeed.SeedUserAsync(userManager);
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
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.UseMiddleware<ExceptionHandlerMiddleware>();
                //app.UseHsts();
            }
            app.UseHttpsRedirection();
            //app.UseStatusCodePagesWithReExecute("/errors/{0}");
            // PictureUrl
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            // Identity
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
