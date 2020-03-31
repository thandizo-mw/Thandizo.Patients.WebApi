using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public class PatientTravelHistoryService : IPatientTravelHistoryService
    {
        private readonly thandizoContext _context;

        public PatientTravelHistoryService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long travelId)
        {
            var patientTravelHistory = await _context.PatientTravelHistory.FirstOrDefaultAsync(x => x.TravelId.Equals(travelId));
           
            var mappedPatientTravelHistory = new AutoMapperHelper<PatientTravelHistory, PatientTravelHistoryDTO>().MapToObject(patientTravelHistory);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientTravelHistory
            };
        }

        public async Task<OutputResponse> Get()
        {
            var patientTravelHistories = await _context.PatientTravelHistory.OrderBy(x => x.PatientId).ToListAsync();

            var mappedPatientTravelHistoryes = new AutoMapperHelper<PatientTravelHistory, PatientTravelHistoryDTO>().MapToList(patientTravelHistories);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientTravelHistoryes
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var travelHistories = await _context.PatientTravelHistory.Where(x => x.TravelId.Equals(patientId))
                .OrderBy(x => x.PatientId)
                .ToListAsync();

            var mappedHistories = new AutoMapperHelper<PatientTravelHistory, PatientTravelHistoryDTO>().MapToList(travelHistories);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHistories
            };
        }

        
    }
}
