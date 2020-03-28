using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientStatusService
    {
        Task<OutputResponse> Add(PatientStatusDTO patientStatus);
        Task<OutputResponse> Delete(int statusId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int statusId);
        Task<OutputResponse> Update(PatientStatusDTO patientStatus);
    }
}