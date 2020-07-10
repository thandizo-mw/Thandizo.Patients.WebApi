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
    public class TransmissionClassificationService : ITransmissionClassificationService
    {
        private readonly thandizoContext _context;

        public TransmissionClassificationService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int classificationId)
        {
            var classification = await _context.TransmissionClassifications.FirstOrDefaultAsync(x => x.ClassificationId.Equals(classificationId));

            var mappedClassification = new AutoMapperHelper<TransmissionClassifications, TransmissionClassificationDTO>().MapToObject(classification);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedClassification
            };
        }

        public async Task<OutputResponse> Get()
        {
            var classifications = await _context.TransmissionClassifications.OrderBy(x => x.ClassificationName).ToListAsync();

            var mappedClassifications = new AutoMapperHelper<TransmissionClassifications, TransmissionClassificationDTO>().MapToList(classifications);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedClassifications
            };
        }

        public async Task<OutputResponse> Add(TransmissionClassificationDTO classification)
        {
            var isFound = await _context.TransmissionClassifications.AnyAsync(
                x => x.ClassificationName.ToLower() == classification.ClassificationName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Classification name already exist, duplicates not allowed"
                };
            }

            var mappedClassification = new AutoMapperHelper<TransmissionClassificationDTO, TransmissionClassifications>().MapToObject(classification);
            mappedClassification.RowAction = "I";
            mappedClassification.DateCreated = DateTime.UtcNow;

            await _context.TransmissionClassifications.AddAsync(mappedClassification);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(TransmissionClassificationDTO classification)
        {
            var classificationToUpdate = await _context.TransmissionClassifications.FirstOrDefaultAsync(
                x => x.ClassificationId.Equals(classification.ClassificationId));

            if (classificationToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Transmission classification specified does not exist, update cancelled"
                };
            }

            //update details
            classificationToUpdate.ClassificationName = classification.ClassificationName;
            classificationToUpdate.RowAction = "U";
            classificationToUpdate.ModifiedBy = classification.CreatedBy;
            classificationToUpdate.DateModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int classificationId)
        {
            //check if there are any records associated with the specified id
            var isFound = await _context.Patients.AnyAsync(x => x.ClassificationId.Equals(classificationId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified transmission classification status has patients attached, deletion denied"
                };
            }

            var classification = await _context.TransmissionClassifications.FirstOrDefaultAsync(
                x => x.ClassificationId.Equals(classificationId));

            if (classification == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Transmission classification specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.TransmissionClassifications.Remove(classification);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
