using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Thandizo.DAL.Models;
using Thandizo.Patients.BLL.Models;

namespace Thandizo.Patients.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEntityFrameworkNpgsql().AddDbContext<thandizoContext>(options =>
                        options.UseNpgsql(Configuration.GetConnectionString("DatabaseConnection")));

            var customConfiguration = new CustomConfiguration
            {
                EmailTemplate = Path.Combine(Environment.ContentRootPath, "MessageTemplates", "email_self_registration.html"),
                SmsTemplate = Path.Combine(Environment.ContentRootPath, "MessageTemplates", "sms_self_registration.txt"),
                SmsQueueAddress = string.Concat(Configuration["RabbitMQHost"], "/", Configuration["SmsQueue"]),
                EmailQueueAddress = string.Concat(Configuration["RabbitMQHost"], "/", Configuration["EmailQueue"]),
                PatientQueueAddress = string.Concat(Configuration["RabbitMQHost"], "/", Configuration["DhisQueue"]),
                DailySymptomsQueueAddress = string.Concat(Configuration["RabbitMQHost"], "/", Configuration["DhisDailySymptomsQueue"]),
                SourceEmailAddress = Configuration["SourceEmailAddress"],
                DailySymptomsEmailSubject = Configuration["DailySymptomsEmailSubject"],
                RegistrationEmailSubject = Configuration["RegistrationEmailSubject"]
            };

            var bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                var host = configure.Host(new Uri(Configuration["RabbitMQHost"]), h =>
                {
                    h.Username(Configuration["RabbitMQUsername"]);
                    h.Password(Configuration["RabbitMQPassword"]);
                });
            });
            services.AddSingleton<IPublishEndpoint>(bus);
            services.AddSingleton<ISendEndpointProvider>(bus);
            services.AddSingleton(bus);
            bus.Start();
            services.AddDomainServices(customConfiguration);
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thandizo Patient API V1");
            });
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod());
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
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
