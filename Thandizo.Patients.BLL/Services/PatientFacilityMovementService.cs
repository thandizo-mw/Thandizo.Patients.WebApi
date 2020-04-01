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
    public class PatientFacilityMovementService : IPatientFacilityMovementService
    {
        private readonly thandizoContext _context;

        public PatientFacilityMovementService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long movementId)
        {
            var movement = await _context.PatientFacilityMovements.Where(x => x.MovementId.Equals(movementId))
                    .Select(x => new PatientFacilityMovementResponse
                    {
                        CreatedBy = x.CreatedBy,
                        DateCreated = x.DateCreated,
                        FromDataCenterId = x.FromDataCenterId,
                        FromDataCenterName = x.FromDataCenter.CenterName,
                        MovementDate = x.MovementDate,
                        MovementId = x.MovementId,
                        PatientId = x.PatientId,
                        ToDataCenterId = x.ToDataCenterId,
                        ToDataCenterName = x.ToDataCenter.CenterName
                    }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = movement
            };
        }

        public async Task<OutputResponse> Get()
        {
            var movements = await _context.PatientFacilityMovements.OrderBy(x => x.PatientId).ThenBy(x => x.MovementDate)
                .Select(x => new PatientFacilityMovementResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    FromDataCenterId = x.FromDataCenterId,
                    FromDataCenterName = x.FromDataCenter.CenterName,
                    MovementDate = x.MovementDate,
                    MovementId = x.MovementId,
                    PatientId = x.PatientId,
                    ToDataCenterId = x.ToDataCenterId,
                    ToDataCenterName = x.ToDataCenter.CenterName
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = movements
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var movements = await _context.PatientFacilityMovements.Where(x => x.PatientId.Equals(patientId))
                .Select(x => new PatientFacilityMovementResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    FromDataCenterId = x.FromDataCenterId,
                    FromDataCenterName = x.FromDataCenter.CenterName,
                    MovementDate = x.MovementDate,
                    MovementId = x.MovementId,
                    PatientId = x.PatientId,
                    ToDataCenterId = x.ToDataCenterId,
                    ToDataCenterName = x.ToDataCenter.CenterName
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = movements
            };
        }

        public async Task<OutputResponse> Add(PatientFacilityMovementDTO movement)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedMovement = new AutoMapperHelper<PatientFacilityMovementDTO, PatientFacilityMovements>().MapToObject(movement);
                mappedMovement.DateCreated = DateTime.UtcNow.AddHours(2);

                await _context.PatientFacilityMovements.AddAsync(mappedMovement);
                await _context.SaveChangesAsync();

                scope.Complete();
            }
            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientFacilityMovementDTO movement)
        {
            var movementToUpdate = await _context.PatientFacilityMovements.FirstOrDefaultAsync(
                    x => x.MovementId.Equals(movement.MovementId));

            if (movementToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient facility movement specified does not exist, update cancelled"
                };
            }

            //update details
            movementToUpdate.FromDataCenterId = movement.FromDataCenterId;
            movementToUpdate.ToDataCenterId = movement.ToDataCenterId;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }
    }
}
