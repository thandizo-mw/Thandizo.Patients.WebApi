using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientHistoryService
    {
        Task<OutputResponse> Add(PatientHistoryDTO patientHistory);
        Task<OutputResponse> Get(long historyId);
        Task<OutputResponse> GetByPatient(long patientId);
    }
}