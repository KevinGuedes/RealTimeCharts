using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RealTimeCharts.Infra.IoC;
using System.Reflection;

namespace RealTimeCharts.Presentation.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRabbitMQBus(Configuration);
            services.AddMediatRToAppHandlers();
            services.AddMediatRToAssemblies(new Assembly[] { 
               Assembly.GetExecutingAssembly()
            });
            services.ConfigureValidators();

            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RealTimeCharts.Presentation.Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealTimeCharts.Presentation.Api v1"));
            }

            app.UseCors(options => options
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials()
                 .WithOrigins("http://localhost:4200"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
