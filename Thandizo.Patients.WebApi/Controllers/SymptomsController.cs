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
    public class SymptomsController : ControllerBase
    {
        ISymptomService _service;

        public SymptomsController(ISymptomService service)
        {
            _service = service;
        }

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] int symptomId)
        {
            var response = await _service.Get(symptomId);

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
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]PatientSymptomDTO symptom)
        {
            var outputHandler = await _service.Add(symptom);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }

        [HttpPut("Update")]
        [ValidateModelState]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]PatientSymptomDTO symptom)
        {
            var outputHandler = await _service.Update(symptom);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]int symptomId)
        {
            var outputHandler = await _service.Delete(symptomId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }

        [HttpGet("GetByStatus")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetByStatus([FromQuery]string valuesFilter)
        {
            var response = await _service.Get(valuesFilter);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }
    }
}