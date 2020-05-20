using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.ApiExtensions.General;
using Thandizo.DataModels.Patients;
using Thandizo.Patients.BLL.Services;

namespace Thandizo.Patients.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientDailyStatusesController : ControllerBase
    {
        IPatientDailyStatusService _service;
        private readonly IConfiguration _configuration;

        public PatientDailyStatusesController(IPatientDailyStatusService service)
        {
            _service = service;
        }

        public string DhisDailySymptomsQueueAddress =>
            string.Concat(_configuration["RabbitMQHost"], "/", _configuration["DhisDailySymptomsQueue"]);

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] long submissionId)
        {
            var response = await _service.Get(submissionId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByPatient")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetByPatient([FromQuery]long patientId)
        {
            var response = await _service.GetByPatient(patientId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }
        
        [HttpGet("GetPatientsByDate")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetPatientsByDate([FromQuery]DateTime fromSubmissionDate, DateTime toSubmissionDate)
        {
            var response = await _service.GetPatientByDate(fromSubmissionDate, toSubmissionDate);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByPatientByDate")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetByPatientByDate([FromQuery]long patientId, DateTime fromSubmissionDate, DateTime toSubmissionDate)
        {
            var response = await _service.GetByPatientByDate(patientId, fromSubmissionDate, toSubmissionDate);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("Add")]
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]IEnumerable<PatientDailyStatusDTO> statuses)
        {
            var outputHandler = await _service.Add(statuses, DhisDailySymptomsQueueAddress);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }
    }
}