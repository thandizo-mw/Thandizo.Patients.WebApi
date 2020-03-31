using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientTravelHistoryService
    {
        Task<OutputResponse> Get(long travelId);
        Task<OutputResponse> Get();
        Task<OutputResponse> GetByPatient(long patientId);
    }
}