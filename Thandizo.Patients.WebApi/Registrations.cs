using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Thandizo.DAL.Models;
using Thandizo.Patients.BLL.Services;
using Thandizo.Patients.WebApi.Models;

namespace Thandizo.Patients.WebApi
{
    public static class Registrations
    {
        /// <summary>
        /// Registers domain services to the specified
        /// service descriptor
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddDomainServices(this IServiceCollection services, MessageTemplateModel messageTemplate)
        {
            services.AddScoped<IPatientService>(x => new PatientService(
                x.GetRequiredService<thandizoContext>(),
                x.GetRequiredService<IBusControl>(),
                messageTemplate.EmailTemplateFilePath,
                messageTemplate.SmsTemplateFilePath));
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IPatientDailyStatusService, PatientDailyStatusService>();
            services.AddScoped<IPatientHistoryService, PatientHistoryService>();
            services.AddScoped<ISymptomService, SymptomService>();
            services.AddScoped<ITransmissionClassificationService, TransmissionClassificationService>();
            services.AddScoped<IPatientTravelHistoryService, PatientTravelHistoryService>();
            services.AddScoped<IPatientLocationMovementService, PatientLocationMovementService>();
            services.AddScoped<IPatientFacilityMovementService, PatientFacilityMovementService>();
            return services.AddScoped<IPatientStatusService, PatientStatusService>();
        }
    }
}