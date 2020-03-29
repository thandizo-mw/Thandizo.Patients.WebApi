using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public class PatientHistoryService : IPatientHistoryService
    {
        private readonly thandizoContext _context;

        public PatientHistoryService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long historyId)
        {
            var history = await _context.PatientHistory.FirstOrDefaultAsync(x => x.HistoryId.Equals(historyId));

            var mappedHistory = new AutoMapperHelper<PatientHistory, PatientHistoryDTO>().MapToObject(history);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHistory
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var histories = await _context.PatientHistory.Where(x => x.PatientId.Equals(patientId))
                .OrderBy(x => x.DateReported)
                .ToListAsync();

            var mappedHistories = new AutoMapperHelper<PatientHistory, PatientHistoryDTO>().MapToList(histories);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHistories
            };
        }

        public async Task<OutputResponse> Add(PatientHistoryDTO patientHistory)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //insert a new patient status
                var mappedHistory = new AutoMapperHelper<PatientHistoryDTO, PatientHistory>().MapToObject(patientHistory);
                mappedHistory.DateCreated = DateTime.UtcNow.AddHours(2);

                await _context.PatientHistory.AddAsync(mappedHistory);

                //update the patient to reflect the current status
                var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId.Equals(patientHistory.PatientId));
                patient.PatientStatusId = patientHistory.PatientStatusId;

                await _context.SaveChangesAsync();
                scope.Complete();
            }

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }
    }
}
