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
    public class PatientLocationMovementService : IPatientLocationMovementService
    {
        private readonly thandizoContext _context;

        public PatientLocationMovementService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(long movementId)
        {
            var movement = await _context.PatientLocationMovements.Where(x => x.MovementId.Equals(movementId))
                .Select(x => new PatientLocationMovementResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    ImagePath = x.ImagePath,
                    MovementDate = x.MovementDate,
                    MovementId = x.MovementId,
                    PatientId = x.PatientId
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = movement
            };
        }

        public async Task<OutputResponse> Get()
        {
            var movements = await _context.PatientLocationMovements.OrderBy(x => x.MovementDate)
                .Select(x => new PatientLocationMovementResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    ImagePath = x.ImagePath,
                    MovementDate = x.MovementDate,
                    MovementId = x.MovementId,
                    PatientId = x.PatientId
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = movements
            };
        }

        public async Task<OutputResponse> GetByPatient(long patientId)
        {
            var movements = await _context.PatientLocationMovements.Where(x => x.PatientId.Equals(patientId))
                .Select(x => new PatientLocationMovementResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    ImagePath = x.ImagePath,
                    MovementDate = x.MovementDate,
                    MovementId = x.MovementId,
                    PatientId = x.PatientId
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = movements
            };
        }

        public async Task<OutputResponse> Add(PatientLocationMovementDTO movement)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedMovement = new AutoMapperHelper<PatientLocationMovementDTO, PatientLocationMovements>().MapToObject(movement);
                mappedMovement.DateCreated = DateTime.UtcNow.AddHours(2);

                await _context.PatientLocationMovements.AddAsync(mappedMovement);
                await _context.SaveChangesAsync();

                scope.Complete();
            }
            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientLocationMovementDTO movement)
        {
            var movementToUpdate = await _context.PatientLocationMovements.FirstOrDefaultAsync(x => x.MovementId.Equals(movement.MovementId));

            if (movementToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient location movement specified does not exist, update cancelled"
                };
            }

            //update details
            movementToUpdate.DistrictCode = movement.DistrictCode;
            movementToUpdate.ImagePath = movement.ImagePath;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }
    }
}
