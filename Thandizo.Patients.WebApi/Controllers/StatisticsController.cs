using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.ApiExtensions.General;
using Thandizo.Patients.BLL.Services;

namespace Thandizo.Patients.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpGet("GetByDistricts")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetByDistricts()
        {
            var response = await _service.GetByDistricts();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByNation")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetByNation()
        {
            var response = await _service.GetByNation();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByRegion")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetByRegion()
        {
            var response = await _service.GetByRegion();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }
    }
}