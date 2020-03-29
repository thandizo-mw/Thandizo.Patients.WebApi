using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientService
    {
        Task<OutputResponse> Add(PatientDTO patient);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(long patientId);
        Task<OutputResponse> Update(PatientDTO patient);
    }
}