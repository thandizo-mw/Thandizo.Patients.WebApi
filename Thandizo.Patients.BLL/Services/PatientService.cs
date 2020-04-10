using MassTransit;
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
using Thandizo.DataModels.Messaging;
using Thandizo.DataModels.Patients;
using Thandizo.DataModels.Patients.Responses;

namespace Thandizo.Patients.BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IBusControl _bus;
        private readonly thandizoContext _context;
        private readonly string _emailTemplate;
        private readonly string _smsTemplate;

        public PatientService(thandizoContext context, IBusControl bus, string emailTemplate, string smsTemplate)
        {
            _bus = bus;
            _context = context;
            _emailTemplate = emailTemplate;
            _smsTemplate = smsTemplate;
        }

        public async Task<OutputResponse> GetByPhoneNumber(string phoneNumber)
        {
            var patient = await _context.Patients.Where(x => x.PhoneNumber.Equals(phoneNumber))
                .Select(x => new PatientResponse
                {
                    ClassificationId = x.ClassificationId,
                    ClassificationName = x.Classification.ClassificationName,
                    CreatedBy = x.CreatedBy,
                    DataCenterId = x.DataCenterId,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DateOfBirth = x.DateOfBirth,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    EmailAddress = x.EmailAddress,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    HomeAddress = x.HomeAddress,
                    IdentificationNumber = x.IdentificationNumber,
                    IdentificationTypeId = x.IdentificationTypeId,
                    IdentitificationTypeName = x.IdentificationType.Description,
                    LastName = x.LastName,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    ModifiedBy = x.ModifiedBy,
                    NationalityCode = x.NationalityCode,
                    NationalityName = x.NationalityCodeNavigation.NationalityName,
                    OtherNames = x.OtherNames,
                    PatientId = x.PatientId,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName,
                    PhoneNumber = x.PhoneNumber,
                    PhysicalAddress = x.PhysicalAddress,
                    RowAction = x.RowAction,
                    IsConfirmed = x.IsConfirmed,
                    SourceId = x.SourceId,
                    SourceName = x.Source.SourceName
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patient
            };
        }

        public async Task<OutputResponse> Get(long patientId)
        {
            var patient = await _context.Patients.Where(x => x.PatientId.Equals(patientId))
                .Select(x => new PatientResponse
                {
                    ClassificationId = x.ClassificationId,
                    ClassificationName = x.Classification.ClassificationName,
                    CreatedBy = x.CreatedBy,
                    DataCenterId = x.DataCenterId,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DateOfBirth = x.DateOfBirth,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    EmailAddress = x.EmailAddress,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    HomeAddress = x.HomeAddress,
                    IdentificationNumber = x.IdentificationNumber,
                    IdentificationTypeId = x.IdentificationTypeId,
                    IdentitificationTypeName = x.IdentificationType.Description,
                    LastName = x.LastName,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    ModifiedBy = x.ModifiedBy,
                    NationalityCode = x.NationalityCode,
                    NationalityName = x.NationalityCodeNavigation.NationalityName,
                    OtherNames = x.OtherNames,
                    PatientId = x.PatientId,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName,
                    PhoneNumber = x.PhoneNumber,
                    PhysicalAddress = x.PhysicalAddress,
                    RowAction = x.RowAction,
                    IsConfirmed = x.IsConfirmed,
                    SourceId = x.SourceId,
                    SourceName = x.Source.SourceName
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patient
            };
        }

        public async Task<OutputResponse> Get()
        {
            var patients = await _context.Patients.OrderBy(x => x.FirstName)
                .Select(x => new PatientResponse
                {
                    ClassificationId = x.ClassificationId,
                    ClassificationName = x.Classification.ClassificationName,
                    CreatedBy = x.CreatedBy,
                    DataCenterId = x.DataCenterId,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DateOfBirth = x.DateOfBirth,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    EmailAddress = x.EmailAddress,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    HomeAddress = x.HomeAddress,
                    IdentificationNumber = x.IdentificationNumber,
                    IdentificationTypeId = x.IdentificationTypeId,
                    IdentitificationTypeName = x.IdentificationType.Description,
                    LastName = x.LastName,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    ModifiedBy = x.ModifiedBy,
                    NationalityCode = x.NationalityCode,
                    NationalityName = x.NationalityCodeNavigation.NationalityName,
                    OtherNames = x.OtherNames,
                    PatientId = x.PatientId,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName,
                    PhoneNumber = x.PhoneNumber,
                    PhysicalAddress = x.PhysicalAddress,
                    RowAction = x.RowAction,
                    IsConfirmed = x.IsConfirmed,
                    SourceId = x.SourceId,
                    SourceName = x.Source.SourceName
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patients
            };
        }

        public async Task<OutputResponse> Add(PatientDTO patient, string emailQueueAddress, string smsQueueAddress)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedPatient = new AutoMapperHelper<PatientDTO, DAL.Models.Patients>().MapToObject(patient);
                mappedPatient.RowAction = "I";
                mappedPatient.DateCreated = DateTime.UtcNow.AddHours(2);

                //if its not data entry (DE), it implies that is self-registration that requires
                //to be confirmed before it becomes a case to avoid bogus reporting
                //and also, set for default values for registration
                if (!mappedPatient.SourceId.ToUpper().Equals("DE"))
                {
                    mappedPatient.IsConfirmed = false;

                    //default mappings
                    var registrationMapping = await _context.RegistrationMappings.FirstOrDefaultAsync();
                    mappedPatient.PatientStatusId = registrationMapping.PatientStatusId;
                    mappedPatient.DataCenterId = registrationMapping.DataCenterId;
                    mappedPatient.ClassificationId = registrationMapping.ClassificationId;
                }

                var addedPatient = await _context.Patients.AddAsync(mappedPatient);
                await _context.SaveChangesAsync();

                var patientHistory = new PatientHistory
                {
                    CreatedBy = patient.CreatedBy,
                    DateCreated = DateTime.UtcNow.AddHours(2),
                    DateReported = DateTime.UtcNow.AddHours(2).Date,
                    PatientId = addedPatient.Entity.PatientId,
                    PatientStatusId = mappedPatient.PatientStatusId
                };
                await _context.PatientHistory.AddAsync(patientHistory);

                var smsEndpoint = await _bus.GetSendEndpoint(new Uri(smsQueueAddress));
                var emailEndpoint = await _bus.GetSendEndpoint(new Uri(emailQueueAddress));

                var phoneNumbers = new List<string>();
                var emailAddresses = new List<string>();
                var responseTeamMappings = _context.ResponseTeamMappings.Include("TeamMember").Where(x => x.DistrictCode.Equals(patient.DistrictCode));

                foreach (var responseTeam in responseTeamMappings)
                {
                    var phoneNumber = responseTeam.TeamMember.PhoneNumber;
                    var emailAddress = responseTeam.TeamMember.EmailAddress;

                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        phoneNumbers.Add(phoneNumber);
                    }

                    if (!string.IsNullOrEmpty(emailAddress))
                    {
                        emailAddresses.Add(emailAddress);
                    }
                }
                await _context.SaveChangesAsync();
                if (phoneNumbers.Any())
                {
                    await smsEndpoint.Send(new MessageModelRequest(new MessageModel
                    {
                        SourceAddress = "Thandizo",
                        DestinationRecipients = phoneNumbers,
                        MessageBody = $"A new patient {patient.FirstName} {patient.LastName} has self registered on the platform. Login to the web portal for more details"
                    }));
                }

                if (emailAddresses.Any())
                {
                    await emailEndpoint.Send(new MessageModelRequest(new MessageModel
                    {
                        SourceAddress = "thandizo@angledimension.com",
                        Subject = "New Patient",
                        DestinationRecipients = emailAddresses,
                        MessageBody = @$"<html>
                                        <body>A new patient {patient.FirstName} {patient.LastName} has self registered on the platform. Login to the web portal for more details
                                        </body>
                                        </html>"
                    }));
                }

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
