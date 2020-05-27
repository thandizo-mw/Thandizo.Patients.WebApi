using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Thandizo.DAL.Models;
using Thandizo.Patients.BLL.Models;
using Thandizo.Patients.BLL.Services;

namespace Thandizo.Patients.WebApi
{
    public static class Registrations
    {
        /// <summary>
        /// Registers domain services to the specified
        /// service descriptor
        /// </summary>
        /// <param name="services"></param>
        /// <param name="customConfiguration"></param>
        public static IServiceCollection AddDomainServices(this IServiceCollection services, CustomConfiguration customConfiguration)
        {
            services.AddScoped<IPatientService>(x => new PatientService(
                x.GetRequiredService<thandizoContext>(),
                x.GetRequiredService<IBusControl>(),
                customConfiguration));
            services.AddScoped<IPatientDailyStatusService>(x => new PatientDailyStatusService(
                x.GetRequiredService<thandizoContext>(),
                x.GetRequiredService<IBusControl>(),
                customConfiguration));
            services.AddScoped<IStatisticsService, StatisticsService>();
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