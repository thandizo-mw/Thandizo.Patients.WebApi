using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.ApiExtensions.General;
using Thandizo.DataModels.Patients;
using Thandizo.Patients.BLL.Services;

namespace Thandizo.Patients.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransmissionClassificationsController : ControllerBase
    {
        ITransmissionClassificationService _service;

        public TransmissionClassificationsController(ITransmissionClassificationService service)
        {
            _service = service;
        }

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] int classificationId)
        {
            var response = await _service.Get(classificationId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetAll")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.Get();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("Add")]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]TransmissionClassificationDTO classification)
        {
            var outputHandler = await _service.Add(classification);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Created("", outputHandler);
        }

        [HttpPut("Update")]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]TransmissionClassificationDTO classification)
        {
            var outputHandler = await _service.Update(classification);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]int classificationId)
        {
            var outputHandler = await _service.Delete(classificationId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }
    }
}