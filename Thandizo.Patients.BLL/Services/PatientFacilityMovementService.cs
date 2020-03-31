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
    public class PatientFacilityMovementService : IPatientFacilityMovementService
    {
        private readonly thandizoContext _context;

        public PatientFacilityMovementService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long facilityMovementId)
        {
            var patientFacilityMovement = await _context.PatientFacilityMovements.FirstOrDefaultAsync(x => x.MovementId.Equals(facilityMovementId));
            var mappedPatientFacilityMovement = new AutoMapperHelper<DAL.Models.PatientFacilityMovements, PatientFacilityMovementDTO>().MapToObject(patientFacilityMovement);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientFacilityMovement
            };
        }

        public async Task<OutputResponse> Get()
        {
            var patientFacilityMovements = await _context.PatientFacilityMovements.OrderBy(x => x.MovementDate).ToListAsync();

            var mappedPatientFacilityMovements = new AutoMapperHelper<PatientFacilityMovements, PatientFacilityMovementDTO>().MapToList(patientFacilityMovements);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientFacilityMovements
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var patientFacilityMovement = await _context.PatientFacilityMovements.Where(x => x.PatientId.Equals(patientId)).ToListAsync();
            var mappedPatientFacilityMovement = new AutoMapperHelper<PatientFacilityMovements, PatientFacilityMovementDTO>().MapToList(patientFacilityMovement);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientFacilityMovement
            };
        }

        public async Task<OutputResponse> Add(PatientFacilityMovementDTO patientFacilityMovement)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedPatientFacilityMovement = new AutoMapperHelper<PatientFacilityMovementDTO, DAL.Models.PatientFacilityMovements>().MapToObject(patientFacilityMovement);
                mappedPatientFacilityMovement.DateCreated = DateTime.UtcNow.AddHours(2);

                var addedPatientFacilityMovement = await _context.PatientFacilityMovements.AddAsync(mappedPatientFacilityMovement);
                await _context.SaveChangesAsync();

                scope.Complete();
            }
            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientFacilityMovementDTO patientFacilityMovement)
        {
            var patientFacilityMovementToUpdate = await _context.PatientFacilityMovements.FirstOrDefaultAsync(x => x.MovementId.Equals(patientFacilityMovement.MovementId));

            if (patientFacilityMovementToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient facility movement specified does not exist, update cancelled"
                };
            }

            //update details
            patientFacilityMovementToUpdate.PatientId = patientFacilityMovement.PatientId;
            patientFacilityMovementToUpdate.FromDataCenterId = patientFacilityMovement.FromDataCenterId;
            patientFacilityMovementToUpdate.ToDataCenterId = patientFacilityMovement.ToDataCenterId;
            patientFacilityMovementToUpdate.MovementDate = patientFacilityMovement.MovementDate;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }
    }
}
