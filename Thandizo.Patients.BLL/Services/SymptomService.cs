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
    public class SymptomService : ISymptomService
    {
        private readonly thandizoContext _context;

        public SymptomService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int symptomId)
        {
            var symptom = await _context.PatientSymptoms.FirstOrDefaultAsync(x => x.SymptomId.Equals(symptomId));

            var mappedSymptom = new AutoMapperHelper<PatientSymptoms, PatientSymptomDTO>().MapToObject(symptom);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedSymptom
            };
        }

        public async Task<OutputResponse> Get()
        {
            var symptoms = await _context.PatientSymptoms.OrderBy(x => x.SymptomName).ToListAsync();

            var mappedSymptoms = new AutoMapperHelper<PatientSymptoms, PatientSymptomDTO>().MapToList(symptoms);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedSymptoms
            };
        }

        public async Task<OutputResponse> Add(PatientSymptomDTO symptom)
        {
            var isFound = await _context.PatientSymptoms.AnyAsync(x => x.SymptomName.ToLower() == symptom.SymptomName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Symptom name already exist, duplicates not allowed"
                };
            }

            var mappedPatientStatus = new AutoMapperHelper<PatientSymptomDTO, PatientSymptoms>().MapToObject(symptom);
            mappedPatientStatus.RowAction = "I";
            mappedPatientStatus.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.PatientSymptoms.AddAsync(mappedPatientStatus);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(PatientSymptomDTO symptom)
        {
            var symptomToUpdate = await _context.PatientSymptoms.FirstOrDefaultAsync(x => x.SymptomId.Equals(symptom.SymptomId));

            if (symptomToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Symptom specified does not exist, update cancelled"
                };
            }

            //update details
            symptomToUpdate.SymptomName = symptom.SymptomName;
            symptomToUpdate.RowAction = "U";
            symptomToUpdate.ModifiedBy = symptom.CreatedBy;
            symptomToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int symptomId)
        {
            //check if there are any records associated with the specified record
            var isFound = await _context.PatientDailyStatuses.AnyAsync(x => x.SymptomId.Equals(symptomId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified symptom has patient daily statuses submission attached, deletion denied"
                };
            }

            var symptom = await _context.PatientSymptoms.FirstOrDefaultAsync(x => x.SymptomId.Equals(symptomId));

            if (symptom == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Symptom specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.PatientSymptoms.Remove(symptom);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
