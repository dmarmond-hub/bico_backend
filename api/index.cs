using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Bico.Models;
using Bico.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Bico.Api
{
    public class VercelFunction
    {
        [HttpFunction]
        public async Task<IActionResult> Run(HttpRequest req)
        {
            var builder = WebApplication.CreateBuilder();
            
            // Add configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            // Add services to the container
            builder.Services.Configure<BicoDatabaseSettings>(
                builder.Configuration.GetSection("BicoDatabase"));

            builder.Services.AddSingleton<UsuariosService>();
            builder.Services.AddSingleton<ContratosService>();
            builder.Services.AddSingleton<EfipayService>();
            builder.Services.AddSingleton<ReviewsService>();

            builder.Services.AddControllers()
                .AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Ry74cBQva5dThwbwchR9jhbtRFnJxWSZ"))
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();

            // Process the request
            await app.RunAsync(req.HttpContext);
            
            return new OkResult();
        }
    }
}