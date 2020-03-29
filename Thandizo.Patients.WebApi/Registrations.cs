using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ISymptomService, SymptomService>();
            services.AddScoped<ITransmissionClassificationService, TransmissionClassificationService>();
            return services.AddScoped<IPatientStatusService, PatientStatusService>();
        }
    }
}
