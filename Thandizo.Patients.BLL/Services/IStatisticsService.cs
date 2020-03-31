using System.Threading.Tasks;
using Thandizo.DataModels.General;

namespace Thandizo.Patients.BLL.Services
{
    public interface IStatisticsService
    {
        Task<OutputResponse> GetByDistricts();
        Task<OutputResponse> GetByNation();
        Task<OutputResponse> GetByRegion();
    }
}