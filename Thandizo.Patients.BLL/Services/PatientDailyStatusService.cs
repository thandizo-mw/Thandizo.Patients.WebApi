using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class PatientDailyStatusService : IPatientDailyStatusService
    {
        private readonly thandizoContext _context;

        public PatientDailyStatusService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long submissionId)
        {
            var dailyStatus = await _context.PatientDailyStatuses.FirstOrDefaultAsync(x => x.SubmissionId.Equals(submissionId));

            var mappedDailyStatus = new AutoMapperHelper<PatientDailyStatuses, PatientDailyStatusDTO>().MapToObject(dailyStatus);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedDailyStatus
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var dailyStatuses = await _context.PatientDailyStatuses.Where(x => x.PatientId.Equals(patientId))
                .OrderBy(x => x.DateSubmitted)
                .ThenBy(x => x.SymptomId)
                .ToListAsync();

            var mappedDailyStatuses = new AutoMapperHelper<PatientDailyStatuses, PatientDailyStatusDTO>().MapToList(dailyStatuses);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedDailyStatuses
            };
        }

        public async Task<OutputResponse> Add(IEnumerable<PatientDailyStatusDTO> statuses)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var status in statuses)
                {
                    var mappedStatus = new AutoMapperHelper<PatientDailyStatusDTO, PatientDailyStatuses>().MapToObject(status);
                    mappedStatus.DateCreated = DateTime.UtcNow.AddHours(2);

                    await _context.PatientDailyStatuses.AddAsync(mappedStatus);
                }

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
