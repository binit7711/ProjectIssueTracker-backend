
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectIssueTracker.Authorization;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Mappings;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace ProjectIssueTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("ApiConnection");
            // builder.Services.AddDbContext<ApiDBContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

            builder.Services.AddDbContext<ApiDBContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));
            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });


            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Secret").Value!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ProjectOwnerPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new ProjectOwnershipRequirement());
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://127.0.0.1:4200", "http://localhost:4200").AllowAnyMethod().AllowAnyHeader());
            });

            builder.Services.AddScoped<IAuthorizationHandler, ProjectOwnershipAuthorization>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}