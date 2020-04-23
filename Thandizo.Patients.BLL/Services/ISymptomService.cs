using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface ISymptomService
    {
        Task<OutputResponse> Add(PatientSymptomDTO symptom);
        Task<OutputResponse> Delete(int symptomId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int symptomId);
        Task<OutputResponse> Update(PatientSymptomDTO symptom);
        Task<OutputResponse> Get(string valuesFilter);
    }
}