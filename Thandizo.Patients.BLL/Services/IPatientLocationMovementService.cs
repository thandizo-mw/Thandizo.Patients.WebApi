﻿using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public interface IPatientLocationMovementService
    {
        Task<OutputResponse> Add(PatientLocationMovementDTO movement);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(long movementId);
        Task<OutputResponse> GetByPatient(long patientId);
        Task<OutputResponse> Update(PatientLocationMovementDTO movement);
    }
}