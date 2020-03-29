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
    public class PatientService : IPatientService
    {
        private readonly thandizoContext _context;

        public PatientService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> GetByPhoneNumber(string phoneNumber)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));
            var mappedPatient = new AutoMapperHelper<DAL.Models.Patients, PatientDTO>().MapToObject(patient);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatient
            };
        }

        public async Task<OutputResponse> Get(long patientId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId.Equals(patientId));
            var mappedPatient = new AutoMapperHelper<DAL.Models.Patients, PatientDTO>().MapToObject(patient);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatient
            };
        }

        public async Task<OutputResponse> Get()
        {
            var patients = await _context.Patients.OrderBy(x => x.FirstName).ToListAsync();

            var mappedPatients = new AutoMapperHelper<DAL.Models.Patients, PatientDTO>().MapToList(patients);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedPatients
            };
        }

        public async Task<OutputResponse> Add(PatientDTO patient)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedPatient = new AutoMapperHelper<PatientDTO, DAL.Models.Patients>().MapToObject(patient);
                mappedPatient.RowAction = "I";
                mappedPatient.DateCreated = DateTime.UtcNow.AddHours(2);

                var addedPatient = await _context.Patients.AddAsync(mappedPatient);
                await _context.SaveChangesAsync();

                var patientHistory = new PatientHistory
                {
                    CreatedBy = patient.CreatedBy,
                    DateCreated = DateTime.UtcNow.AddHours(2),
                    DateReported = DateTime.UtcNow.AddHours(2).Date,
                    PatientId = addedPatient.Entity.PatientId,
                    PatientStatusId = patient.PatientStatusId
                };
                await _context.PatientHistory.AddAsync(patientHistory);
                await _context.SaveChangesAsync();

                scope.Complete();
            }
            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientDTO patient)
        {
            var patientToUpdate = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId.Equals(patient.PatientId));

            if (patientToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient specified does not exist, update cancelled"
                };
            }

            //update details
            patientToUpdate.FirstName = patient.FirstName;
            patientToUpdate.OtherNames = patient.OtherNames;
            patientToUpdate.LastName = patient.LastName;
            patientToUpdate.ClassificationId = patient.ClassificationId;
            patientToUpdate.DataCenterId = patient.DataCenterId;
            patientToUpdate.DateOfBirth = patient.DateOfBirth;
            patientToUpdate.DistrictCode = patient.DistrictCode;
            patientToUpdate.EmailAddress = patient.EmailAddress;
            patientToUpdate.Gender = patient.Gender;
            patientToUpdate.HomeAddress = patient.HomeAddress;
            patientToUpdate.IdentificationNumber = patient.IdentificationNumber;
            patientToUpdate.IdentificationTypeId = patient.IdentificationTypeId;
            patientToUpdate.NationalityCode = patient.NationalityCode;
            patientToUpdate.PatientStatusId = patient.PatientStatusId;
            patientToUpdate.PhoneNumber = patient.PhoneNumber;
            patientToUpdate.PhysicalAddress = patient.PhysicalAddress;
            patientToUpdate.RowAction = "U";
            patientToUpdate.ModifiedBy = patient.CreatedBy;
            patientToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }
    }
}
