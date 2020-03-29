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
    public class PatientsController : ControllerBase
    {
        IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet("GetByPhoneNumber")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetByPhoneNumber([FromQuery] string phoneNumber)
        {
            var response = await _service.GetByPhoneNumber(phoneNumber);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] long patientId)
        {
            var response = await _service.Get(patientId);

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
        public async Task<IActionResult> Add([FromBody]PatientDTO patient)
        {
            var outputHandler = await _service.Add(patient);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Created("", outputHandler);
        }

        [HttpPut("Update")]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]PatientDTO patient)
        {
            var outputHandler = await _service.Update(patient);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }
    }
}