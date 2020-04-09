using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IPatientService _service;
        private readonly IConfiguration _configuration;

        public PatientsController(IPatientService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
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
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]PatientDTO patient)
        {
            
            var smsQueueAddress = string.Concat(_configuration["RabbitMQHost"], "/", _configuration["SmsQueue"]);
            var outputHandler = await _service.Add(patient, smsQueueAddress: smsQueueAddress);

            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }

        [HttpPut("Update")]
        [ValidateModelState]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]PatientDTO patient)
        {
            var outputHandler = await _service.Update(patient);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }
    }
}