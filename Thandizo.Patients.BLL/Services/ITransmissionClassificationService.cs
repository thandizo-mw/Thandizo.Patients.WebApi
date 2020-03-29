using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface ITransmissionClassificationService
    {
        Task<OutputResponse> Add(TransmissionClassificationDTO classification);
        Task<OutputResponse> Delete(int classificationId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int classificationId);
        Task<OutputResponse> Update(TransmissionClassificationDTO classification);
    }
}