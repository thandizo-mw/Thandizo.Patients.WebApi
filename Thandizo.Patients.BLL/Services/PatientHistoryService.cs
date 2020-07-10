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
using Thandizo.DataModels.Patients.Responses;

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
            var history = await _context.PatientHistory.Where(x => x.HistoryId.Equals(historyId))
                .Select(x => new PatientHistoryResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateReported = x.DateReported,
                    HistoryId = x.HistoryId,
                    PatientId = x.PatientId,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = history
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var histories = await _context.PatientHistory.Where(x => x.PatientId.Equals(patientId))
                .OrderBy(x => x.DateReported)
                .Select(x => new PatientHistoryResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateReported = x.DateReported,
                    HistoryId = x.HistoryId,
                    PatientId = x.PatientId,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = histories
            };
        }

        public async Task<OutputResponse> Add(PatientHistoryDTO patientHistory)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //insert a new patient status
                var mappedHistory = new AutoMapperHelper<PatientHistoryDTO, PatientHistory>().MapToObject(patientHistory);
                mappedHistory.DateCreated = DateTime.UtcNow;

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
