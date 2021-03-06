﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientDailyStatusService
    {
        Task<OutputResponse> Add(IEnumerable<PatientDailyStatusDTO> statuses);
        Task<OutputResponse> Get(long submissionId);
        Task<OutputResponse> GetByPatient(long patientId);         
        Task<OutputResponse> GetByPatientByDate(long patientId, DateTime fromSubmittedDate, DateTime toSubmittedDate);
        Task<OutputResponse> GetSymptomStatisticsByDate(DateTime fromSubmittedDate, DateTime toSubmittedDate);
        Task<OutputResponse> GetPatientSymptomStatsByDate(DateTime fromSubmittedDate, DateTime toSubmittedDate);
    }
}