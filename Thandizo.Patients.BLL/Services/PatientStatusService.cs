using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Patients;

namespace Thandizo.Patients.BLL.Services
{
    public class PatientStatusService : IPatientStatusService
    {
        private readonly thandizoContext _context;

        public PatientStatusService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int statusId)
        {
            var patientStatus = await _context.PatientStatuses.FirstOrDefaultAsync(x => x.PatientStatusId.Equals(statusId));
           
            var mappedPatientStatus = new AutoMapperHelper<PatientStatuses, PatientStatusDTO>().MapToObject(patientStatus);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientStatus
            };
        }

        public async Task<OutputResponse> Get()
        {
            var patientStatuses = await _context.PatientStatuses.OrderBy(x => x.PatientStatusName).ToListAsync();

            var mappedPatientStatuses = new AutoMapperHelper<PatientStatuses, PatientStatusDTO>().MapToList(patientStatuses);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatientStatuses
            };
        }

        public async Task<OutputResponse> Add(PatientStatusDTO patientStatus)
        {
            var isFound = await _context.PatientStatuses.AnyAsync(x => x.PatientStatusName.ToLower() == patientStatus.PatientStatusName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient status name already exist, duplicates not allowed"
                };
            }

            var mappedPatientStatus = new AutoMapperHelper<PatientStatusDTO, PatientStatuses>().MapToObject(patientStatus);
            mappedPatientStatus.RowAction = "I";
            mappedPatientStatus.DateCreated = DateTime.UtcNow;

            await _context.PatientStatuses.AddAsync(mappedPatientStatus);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientStatusDTO patientStatus)
        {
            var patientStatusToUpdate = await _context.PatientStatuses.FirstOrDefaultAsync(x => x.PatientStatusId.Equals(patientStatus.PatientStatusId));

            if (patientStatusToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient status specified does not exist, update cancelled"
                };
            }

            //update details
            patientStatusToUpdate.PatientStatusName = patientStatus.PatientStatusName;
            patientStatusToUpdate.Severity = patientStatus.Severity;
            patientStatusToUpdate.RowAction = "U";
            patientStatusToUpdate.ModifiedBy = patientStatus.CreatedBy;
            patientStatusToUpdate.DateModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int statusId)
        {
            //check if there are any records associated with the specified status
            var isFound = await _context.PatientHistory.AnyAsync(x => x.PatientStatusId.Equals(statusId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified patient status has patient history entries attached, deletion denied"
                };
            }

            var patientStatus = await _context.PatientStatuses.FirstOrDefaultAsync(x => x.PatientStatusId.Equals(statusId));

            if (patientStatus == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient status specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.PatientStatuses.Remove(patientStatus);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
