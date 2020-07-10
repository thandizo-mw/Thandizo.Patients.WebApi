using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
using Thandizo.Patients.BLL.Models;

namespace Thandizo.Patients.BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IBusControl _bus;
        private readonly thandizoContext _context;
        private readonly CustomConfiguration _customConfiguration;
        private readonly string _smsTemplate;

        public PatientService(thandizoContext context, IBusControl bus, CustomConfiguration customConfiguration)
        {
            _bus = bus;
            _context = context;
            _customConfiguration = customConfiguration;
        }

        public async Task<OutputResponse> GetByPhoneNumber(string phoneNumber)
        {
            var patients = await _context.Patients.Where(x => x.PhoneNumber.Equals(phoneNumber))
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
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
                    SourceName = x.Source.SourceName,
                    IsSelfRegistered = x.IsSelfRegistered,
                    NextOfKinFirstName = x.NextOfKinFirstName,
                    NextOfKinLastName = x.NextOfKinLastName,
                    NextOfKinPhoneNumber = x.NextOfKinPhoneNumber,
                    ResidenceCountryCode = x.ResidenceCountryCode,
                    CountryName = x.ResidenceCountryCodeNavigation.CountryName
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patients
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
                    SourceName = x.Source.SourceName,
                    IsSelfRegistered = x.IsSelfRegistered,
                    NextOfKinFirstName = x.NextOfKinFirstName,
                    NextOfKinLastName = x.NextOfKinLastName,
                    NextOfKinPhoneNumber = x.NextOfKinPhoneNumber,
                    ResidenceCountryCode = x.ResidenceCountryCode,
                    CountryName = x.ResidenceCountryCodeNavigation.CountryName
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
                    SourceName = x.Source.SourceName,
                    IsSelfRegistered = x.IsSelfRegistered,
                    NextOfKinFirstName = x.NextOfKinFirstName,
                    NextOfKinLastName = x.NextOfKinLastName,
                    NextOfKinPhoneNumber = x.NextOfKinPhoneNumber,
                    ResidenceCountryCode = x.ResidenceCountryCode,
                    CountryName = x.ResidenceCountryCodeNavigation.CountryName
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patients
            };
        }

        public async Task<OutputResponse> Add(PatientRequest request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var mappedPatient = new AutoMapperHelper<PatientDTO, DAL.Models.Patients>().MapToObject(request.Patient);
                mappedPatient.RowAction = "I";
                mappedPatient.DateCreated = DateTime.UtcNow;

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
                    CreatedBy = request.Patient.CreatedBy,
                    DateCreated = DateTime.UtcNow,
                    DateReported = DateTime.UtcNow.Date,
                    PatientId = addedPatient.Entity.PatientId,
                    PatientStatusId = mappedPatient.PatientStatusId
                };
                await _context.PatientHistory.AddAsync(patientHistory);

                //add the specified symptoms
                foreach (var status in request.PatientDailyStatuses)
                {
                    var mappedStatus = new AutoMapperHelper<PatientDailyStatusDTO, PatientDailyStatuses>().MapToObject(status);
                    mappedStatus.DateCreated = DateTime.UtcNow;
                    mappedStatus.DateSubmitted = DateTime.UtcNow.Date;
                    mappedStatus.PatientId = addedPatient.Entity.PatientId;

                    await _context.PatientDailyStatuses.AddAsync(mappedStatus);
                }

                var smsEndpoint = await _bus.GetSendEndpoint(new Uri(_customConfiguration.SmsQueueAddress));
                var emailEndpoint = await _bus.GetSendEndpoint(new Uri(_customConfiguration.EmailQueueAddress));
                var dhisEndpoint = await _bus.GetSendEndpoint(new Uri(_customConfiguration.PatientQueueAddress));

                var phoneNumbers = new List<string>();
                var emailAddresses = new List<string>();
                var responseTeamMappings = _context.ResponseTeamMappings.Include("TeamMember")
                    .Where(x => x.DistrictCode.Equals(request.Patient.DistrictCode));

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
                var fullName = string.Concat(request.Patient.FirstName, " ", request.Patient.OtherNames, " ", request.Patient.LastName);


                if (phoneNumbers.Any() && File.Exists(_smsTemplate))
                {
                    var sms = await File.ReadAllTextAsync(_smsTemplate);
                    sms = sms.Replace("{{FULL_NAME}}", fullName);
                    sms = sms.Replace("{{PHONE_NUMBER}}", string.Concat("+", request.Patient.PhoneNumber));
                    await smsEndpoint.Send(new MessageModelRequest(new MessageModel
                    {
                        SourceAddress = "Thandizo",
                        DestinationRecipients = phoneNumbers,
                        MessageBody = sms
                    }));
                }

                if (emailAddresses.Any() && File.Exists(_customConfiguration.EmailTemplate))
                {
                    var district = await _context.Districts.FindAsync(request.Patient.DistrictCode);
                    var registrationSource = await _context.RegistrationSources.FindAsync(request.Patient.SourceId);
                    var email = await File.ReadAllTextAsync(_customConfiguration.EmailTemplate);
                    email = email.Replace("{{REGISTRATION_SOURCE}}", registrationSource.SourceName);
                    email = email.Replace("{{FULL_NAME}}", fullName);
                    email = email.Replace("{{PHONE_NUMBER}}", string.Concat("+", request.Patient.PhoneNumber));
                    email = email.Replace("{{PHYSICAL_ADDRESS}}", request.Patient.PhysicalAddress);
                    email = email.Replace("{{DISTRICT_NAME}}", district.DistrictName);
                    await emailEndpoint.Send(new MessageModelRequest(new MessageModel
                    {
                        SourceAddress = _customConfiguration.SourceEmailAddress,
                        Subject = _customConfiguration.RegistrationEmailSubject,
                        DestinationRecipients = emailAddresses,
                        MessageBody = email
                    }));
                }

                //for DHIS2 integration
                await dhisEndpoint.Send(new DhisPatientModelRequest(addedPatient.Entity.PatientId));

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
            patientToUpdate.NextOfKinPhoneNumber = patient.NextOfKinPhoneNumber;
            patientToUpdate.NextOfKinLastName = patient.NextOfKinLastName;
            patientToUpdate.NextOfKinFirstName = patient.NextOfKinFirstName;
            patientToUpdate.ResidenceCountryCode = patient.ResidenceCountryCode;
            patientToUpdate.RowAction = "U";
            patientToUpdate.ModifiedBy = patient.CreatedBy;
            patientToUpdate.DateModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> ConfirmPatient(long patientId)
        {
            var patientToUpdate = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId.Equals(patientId));

            if (patientToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Patient specified does not exist, update cancelled"
                };
            }


            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //update isConfirmed field
                patientToUpdate.IsConfirmed = true;
                patientToUpdate.DateModified = DateTime.UtcNow;
                patientToUpdate.RowAction = "U";
                await _context.SaveChangesAsync();

                var isFound = await _context.ConfirmedPatients.AnyAsync(x => x.PatientId == patientId);
                if (isFound)
                {
                    return new OutputResponse
                    {
                        IsErrorOccured = true,
                        Message = "Patient already confirmed, duplicates not allowed"
                    };
                }

                var confirmedPatient = new ConfirmedPatientDTO
                {
                    PatientId = patientToUpdate.PatientId,
                    FirstName = $"{patientToUpdate.FirstName} {patientToUpdate.OtherNames}",
                    SurName = patientToUpdate.LastName,
                    DateOfBirth = patientToUpdate.DateOfBirth,
                    Gender = patientToUpdate.Gender,
                    CountryCode = patientToUpdate.NationalityCode,
                    IdentificationType = $"{patientToUpdate.IdentificationTypeId}",
                    IdentificationNumber = patientToUpdate.IdentificationNumber,
                    PhoneNumber = patientToUpdate.PhoneNumber
                };

                var mappedConfirmedPatient = new AutoMapperHelper<ConfirmedPatientDTO, ConfirmedPatients>().MapToObject(confirmedPatient);

                await _context.ConfirmedPatients.AddAsync(mappedConfirmedPatient);
                await _context.SaveChangesAsync();

                scope.Complete();

                return new OutputResponse
                {
                    IsErrorOccured = false,
                    Message = MessageHelper.UpdateSuccess
                };

            }

        }

        public async Task<OutputResponse> GetByResponseTeamMember(string phoneNumber, string valuesFilter)
        {
            valuesFilter = string.IsNullOrEmpty(valuesFilter) ? "true,false" : valuesFilter;

            var statuses = valuesFilter.Split(char.Parse(","));

            List<bool> statusesList = new List<bool>();
            for (int i = 0; i < statuses.Length; i++)
            {
                statusesList.Add(bool.Parse(statuses[i]));
            }

            var patients = await (from x in _context.Patients
                                  join mp in _context.ResponseTeamMappings on x.DistrictCode equals mp.DistrictCode
                                  where mp.TeamMember.PhoneNumber.Equals(phoneNumber)
                                  && statusesList.Contains(x.IsConfirmed)
                                  select new PatientResponse
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
                                      SourceName = x.Source.SourceName,
                                      IsSelfRegistered = x.IsSelfRegistered,
                                      NextOfKinFirstName = x.NextOfKinFirstName,
                                      NextOfKinLastName = x.NextOfKinLastName,
                                      NextOfKinPhoneNumber = x.NextOfKinPhoneNumber,
                                      ResidenceCountryCode = x.ResidenceCountryCode,
                                      CountryName = x.ResidenceCountryCodeNavigation.CountryName
                                  }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patients
            };
        }


        public async Task<OutputResponse> GetPatientsByDate(DateTime fromSubmittedDate, DateTime toSubmittedDate)
        {
            var patients = await _context.PatientDailyStatuses.Where(x => x.DateSubmitted >= fromSubmittedDate.AddHours(-2) && x.DateSubmitted < toSubmittedDate)
                              .GroupBy(x => new
                              {
                                  x.PatientId,
                                  x.Patient.FirstName,
                                  x.Patient.OtherNames,
                                  x.Patient.LastName,
                                  x.Patient.IdentificationNumber
                              }).Select(x => new PatientDTO
                              {
                                  PatientId = x.Key.PatientId,
                                  FirstName = x.Key.FirstName,
                                  OtherNames = x.Key.OtherNames,
                                  LastName = x.Key.LastName,
                                  IdentificationNumber = x.Key.IdentificationNumber
                              }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patients
            };
        }

        public async Task<OutputResponse> GetUnSubmittedPatientsByDate(DateTime fromSubmittedDate, DateTime toSubmittedDate)
        {
            var patients = await (from p in _context.Patients 
                                  join s in (_context.PatientDailyStatuses.Where(x => x.DateSubmitted >= fromSubmittedDate.AddHours(-2) && x.DateSubmitted < toSubmittedDate)
                              .GroupBy(x => new
                              {
                                  x.PatientId,
                                  x.Patient.FirstName,
                                  x.Patient.OtherNames,
                                  x.Patient.LastName,
                                  x.Patient.IdentificationNumber
                              }).Select(x => new PatientDTO
                              {
                                  PatientId = x.Key.PatientId,
                                  FirstName = x.Key.FirstName,
                                  OtherNames = x.Key.OtherNames,
                                  LastName = x.Key.LastName,
                                  IdentificationNumber = x.Key.IdentificationNumber
                              })) on p.PatientId equals s.PatientId into rs
                                  from sub in rs.DefaultIfEmpty()
                                  where sub.PatientId == null
                                  select new { 
                                      p.PatientId,
                                      p.FirstName,
                                      p.OtherNames,
                                      p.LastName,
                                      p.IdentificationNumber
                                  }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = patients
            };
        }

        
    }
}