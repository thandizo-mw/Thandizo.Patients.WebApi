﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.Contracts;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;
using Thandizo.DataModels.Patients.Responses;
using Thandizo.Patients.BLL.Models;
using Thandizo.DataModels.Statistics;

namespace Thandizo.Patients.BLL.Services
{
    public class PatientDailyStatusService : IPatientDailyStatusService
    {
        private readonly thandizoContext _context;
        private readonly IBusControl _bus;
        private readonly CustomConfiguration _customConfiguration;

        public PatientDailyStatusService(thandizoContext context
            , IBusControl bus,
            CustomConfiguration customConfiguration)
        {
            _context = context;
            _bus = bus;
            _customConfiguration = customConfiguration;
        }

        public async Task<OutputResponse> Get(long submissionId)
        {
            var dailyStatus = await _context.PatientDailyStatuses.Where(x => x.SubmissionId.Equals(submissionId))
                              .Select(x => new PatientDailyStatusResponse
                              {
                                  CreatedBy = x.CreatedBy,
                                  SymptomName = x.Symptom.SymptomName,
                                  DateCreated = x.DateCreated,
                                  DateSubmitted = x.DateSubmitted,
                                  PatientId = x.PatientId,
                                  SubmissionId = x.SubmissionId,
                                  SymptomId = x.SymptomId
                              }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dailyStatus
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var dailyStatuses = await _context.PatientDailyStatuses.Where(x => x.PatientId.Equals(patientId))
                .OrderBy(x => x.DateSubmitted)
                .ThenBy(x => x.SymptomId)
                .Select(x => new PatientDailyStatusResponse
                {
                    CreatedBy = x.CreatedBy,
                    SymptomName = x.Symptom.SymptomName,
                    DateCreated = x.DateCreated,
                    DateSubmitted = x.DateSubmitted,
                    PatientId = x.PatientId,
                    SubmissionId = x.SubmissionId,
                    SymptomId = x.SymptomId
                })
                .ToListAsync();


            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dailyStatuses
            };
        }

        public async Task<OutputResponse> GetByPatientByDate(long patientId, DateTime fromSubmittedDate, DateTime toSubmittedDate)
        {
            var dailyStatuses = await _context.PatientDailyStatuses.Where(x => x.DateSubmitted >= fromSubmittedDate.AddHours(-2) 
                                && x.DateSubmitted < toSubmittedDate).Where(x => x.PatientId.Equals(patientId))
                                .OrderBy(x => x.SymptomId)
                                .Select(x => new PatientDailyStatusResponse
                                {
                                    CreatedBy = x.CreatedBy,
                                    SymptomName = x.Symptom.SymptomName,
                                    DateCreated = x.DateCreated,
                                    DateSubmitted = x.DateSubmitted,
                                    PatientId = x.PatientId,
                                    SubmissionId = x.SubmissionId,
                                    SymptomId = x.SymptomId
                                })
                                .ToListAsync();


            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dailyStatuses
            };
        }
        public async Task<OutputResponse> GetSymptomStatisticsByDate(DateTime fromSubmittedDate, DateTime toSubmittedDate)
        {
            var dailyStatusesStaistics = await _context.PatientDailyStatuses.Where(x => x.DateSubmitted >= fromSubmittedDate.AddHours(-2) && x.DateSubmitted < toSubmittedDate)
                                .GroupBy(x => new
                                {
                                    x.Symptom.SymptomName
                                })
                                .Select(x => new SymptomStatisticsDTO
                                {
                                    TotalNumberOfReports = x.Count(),
                                    SymptomName = x.Key.SymptomName
                                })
                                .ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dailyStatusesStaistics
            };
        }
        public async Task<OutputResponse> GetPatientSymptomStatsByDate(DateTime fromSubmittedDate, DateTime toSubmittedDate)
        {
            var dailyPatientStatusesStaistics = await _context.PatientDailyStatuses.Where(x => x.DateSubmitted >= fromSubmittedDate.AddHours(-2) && x.DateSubmitted < toSubmittedDate)
                                .GroupBy(x => new
                                {
                                    x.PatientId,
                                    x.Symptom.SymptomName
                                }).Select(x => new
                                {
                                    x.Key.PatientId,
                                    x.Key.SymptomName
                                })
                                .GroupBy(x => new
                                {
                                    x.SymptomName
                                })
                                .Select(x => new SymptomStatisticsDTO
                                {
                                    TotalNumberOfReports = x.Count(),
                                    SymptomName = x.Key.SymptomName
                                })
                                .ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dailyPatientStatusesStaistics
            };
        }

        public async Task<OutputResponse> Add(IEnumerable<PatientDailyStatusDTO> statuses)
        {
            var submissionDate = DateTime.UtcNow.Date;
            var symptomsToSubmit = statuses.Select(x => x.SymptomId);
            var patientId = statuses.FirstOrDefault().PatientId;

            //check if the status(es) have been submitted already for the day to avoid duplicates
            var submittedSymptoms = await _context.PatientDailyStatuses.Where(x => x.DateSubmitted.Equals(submissionDate)
                     && symptomsToSubmit.Contains(x.SymptomId) && x.PatientId.Equals(patientId))
                    .Select(x => x.Symptom.SymptomName).ToArrayAsync();

            if (submittedSymptoms.Length > 0)
            {
                var message = string.Join(", ", submittedSymptoms);

                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = $"You have already submitted the following symptoms for { string.Format("{0:dd-MMM-yyyy}", submissionDate) }: { message }"
                };
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var status in statuses)
                {
                    var mappedStatus = new AutoMapperHelper<PatientDailyStatusDTO, PatientDailyStatuses>().MapToObject(status);
                    mappedStatus.DateCreated = DateTime.UtcNow;
                    mappedStatus.DateSubmitted = submissionDate;
                    mappedStatus.IsPostedToDhis = false;
                    await _context.PatientDailyStatuses.AddAsync(mappedStatus);
                }

                await _context.SaveChangesAsync();
                scope.Complete();
            }

            //for DHIS2 integration
            var dhisEndpoint = await _bus.GetSendEndpoint(new Uri(_customConfiguration.DailySymptomsQueueAddress));
            await dhisEndpoint.Send(new DhisPatientDailyStatusRequest(statuses));

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }
    }
}
