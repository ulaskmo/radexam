using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rad302feCL2025;
using Rad302feCL2025.Repositories;
using Rad302feWebAPI2025.DataLayer;
using System.Text;
using Tracker.WebAPIClient;

namespace Rad302feWebAPI2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ActivityAPIClient.Track(StudentID: "S219971", StudentName: "ulas", activityName: "Rad302 fe March 2025", Task: "Seeding Application User");

            // For CORS on localhost
            string LocalAllowSpecificOrigins = "_localAllowSpecificOrigins";

            // Add DbContext with SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("IdentityModelConnection")));

            // Add MovieContext with SQLite
            builder.Services.AddDbContext<MovieContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("BusinessModelConnection")));

            // Register the repository factory
            builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie().AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Tokens:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Tokens:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"]))
                };
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rad302 Final Exam API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization Header using Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    }, new String[] {}
                }
                });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: LocalAllowSpecificOrigins,
                                  builder =>
                                  {
                                      // current Blazor localhost endpoints See launchSettings.json in Blazor app
                                      builder.WithOrigins("https://localhost:7141")
                                                            .AllowAnyHeader()
                                                            .AllowAnyMethod();
                                  });
            });
            var app = builder.Build();

            // Create and seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    
                    // Ensure database is created
                    context.Database.EnsureCreated();
                    
                    // Seed the database
                    SeedData.Initialize(context, userManager, roleManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseRouting();
            app.UseCors(LocalAllowSpecificOrigins);
            app.UseAuthorization();
            app.MapControllers();

            
            app.Run();
        }
    }
}
