using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientService
    {
        Task<OutputResponse> Add(PatientRequest request);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(long patientId);
        Task<OutputResponse> Update(PatientDTO patient);
        Task<OutputResponse> GetByPhoneNumber(string phoneNumber);
        Task<OutputResponse> ConfirmPatient(long patientId);
        Task<OutputResponse> GetByResponseTeamMember(string phoneNumber, string valuesFilter);
    }
}