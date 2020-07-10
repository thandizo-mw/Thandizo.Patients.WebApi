using Microsoft.AspNetCore.Mvc;
using System;
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
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]PatientRequest request)
        {
            var outputHandler = await _service.Add(request);

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

        [HttpGet("GetByResponseTeamMember")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetByResponseTeamMember([FromQuery] string phoneNumber,
            [FromQuery]string valuesFilter)
        {
            var response = await _service.GetByResponseTeamMember(phoneNumber, valuesFilter);

            if (response.IsErrorOccured)
            {
                var err = response.Message;
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }


        [HttpGet("GetPatientsByDate")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetPatientsByDate([FromQuery]DateTime fromSubmissionDate, DateTime toSubmissionDate)
        {
            var response = await _service.GetPatientsByDate(fromSubmissionDate, toSubmissionDate);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }
        
        [HttpGet("GetUnSubmittedPatientsByDate")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetUnSubmittedPatientsByDate([FromQuery]DateTime fromSubmissionDate, DateTime toSubmissionDate)
        {
            var response = await _service.GetUnSubmittedPatientsByDate(fromSubmissionDate, toSubmissionDate);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPut("ConfirmPatient")]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> ConfirmPatient([FromBody]long patientId)
        {
            var outputHandler = await _service.ConfirmPatient(patientId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }
    }
}