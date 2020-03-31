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
    public class PatientLocationMovementService : IPatientLocationMovementService
    {
        private readonly thandizoContext _context;

        public PatientLocationMovementService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long locationMovementId)
        {
            var patientLocationMovement = await _context.PatientLocationMovements.FirstOrDefaultAsync(x => x.MovementId.Equals(locationMovementId));
            var mappedPatientLocationMovement = new AutoMapperHelper<DAL.Models.PatientLocationMovements, PatientLocationMovementDTO>().MapToObject(patientLocationMovement);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientLocationMovement
            };
        }

        public async Task<OutputResponse> Get()
        {
            var patientLocationMovements = await _context.PatientLocationMovements.OrderBy(x => x.MovementDate).ToListAsync();

            var mappedPatientLocationMovements = new AutoMapperHelper<PatientLocationMovements, PatientLocationMovementDTO>().MapToList(patientLocationMovements);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientLocationMovements
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var patientLocationMovement = await _context.PatientLocationMovements.Where(x => x.PatientId.Equals(patientId)).ToListAsync();
            var mappedPatientLocationMovement = new AutoMapperHelper<PatientLocationMovements, PatientLocationMovementDTO>().MapToList(patientLocationMovement);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientLocationMovement
            };
        }

        public async Task<OutputResponse> Add(PatientLocationMovementDTO patientLocationMovement)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedPatientLocationMovement = new AutoMapperHelper<PatientLocationMovementDTO, PatientLocationMovements>().MapToObject(patientLocationMovement);
                mappedPatientLocationMovement.DateCreated = DateTime.UtcNow.AddHours(2);

                var addedPatientLocationMovement = await _context.PatientLocationMovements.AddAsync(mappedPatientLocationMovement);
                await _context.SaveChangesAsync();

                scope.Complete();
            }
            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientLocationMovementDTO patientLocationMovement)
        {
            var patientLocationMovementToUpdate = await _context.PatientLocationMovements.FirstOrDefaultAsync(x => x.MovementId.Equals(patientLocationMovement.MovementId));

            if (patientLocationMovementToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient location movement specified does not exist, update cancelled"
                };
            }

            //update details
            patientLocationMovementToUpdate.PatientId = patientLocationMovement.PatientId;
            patientLocationMovementToUpdate.DistrictCode = patientLocationMovement.DistrictCode;
            patientLocationMovementToUpdate.ImagePath = patientLocationMovement.ImagePath;
            patientLocationMovementToUpdate.MovementDate = patientLocationMovement.MovementDate;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }
    }
}
