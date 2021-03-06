﻿using Microsoft.AspNetCore.Mvc;
using System;
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

        public PatientDailyStatusesController(IPatientDailyStatusService service)
        {
            _service = service;
        }

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
        [HttpGet("GetSymptomStatisticsByDate")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetSymptomStatisticsByDate([FromQuery] DateTime fromSubmissionDate, DateTime toSubmissionDate)
        {
            var response = await _service.GetSymptomStatisticsByDate(fromSubmissionDate, toSubmissionDate);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }
        [HttpGet("GetPatientSymptomStatsByDate")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetPatientSymptomStatsByDate([FromQuery] DateTime fromSubmissionDate, DateTime toSubmissionDate)
        {
            var response = await _service.GetPatientSymptomStatsByDate(fromSubmissionDate, toSubmissionDate);

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
            var outputHandler = await _service.Add(statuses);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }
    }
}