using CustomerOnboard.Application.Services;
using CustomerOnboard.Infrastructure.Data;
using CustomerOnboard.Infrastructure.Repositories;
using CustomerOnboarding.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.OpenApi.Models;


namespace CustomerOnboard.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Customer Onboarding API",
                    Version = "v1",
                    Description = "API for managing customer onboarding process"
                });
            });
           builder.Services.AddDbContext<CustomerDbContext>(options =>
                 options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"),
                 b => b.MigrationsAssembly("CustomerOnboard.Infrastructure"))
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine));

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<CustomerService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Onboarding API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}