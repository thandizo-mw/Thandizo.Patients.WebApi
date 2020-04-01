using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Thandizo.DAL.Models;

namespace Thandizo.Patients.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEntityFrameworkNpgsql().AddDbContext<thandizoContext>(options =>
                        options.UseNpgsql(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddDomainServices();

            //Disable automatic model state validation to provide cleaner error messages to avoid default complex object
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Thandizo Patients API",
                    Description = "Patients API for Thandizo platform",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "COVID-19 Malawi Tech Response", Email = "thandizo.mw@gmail.com", Url = new Uri("https://www.thandizo.mw") }
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //this is not needed in PRODUCTION but only in Hosted Testing Environment
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thandizo Khusa API V1");
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetXmlCommentsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Thandizo.Patients.WebApi.xml");
        }
    }
}
