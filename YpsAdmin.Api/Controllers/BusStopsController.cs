using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.BusStop;
using YpsAdmin.Domain.Features.BusStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/bus-stops")]
    [ApiController]
    public class BusStopsController : ControllerBase
    {
        private readonly IBusStopService _busStopService;

        public BusStopsController(IBusStopService busStopService)
        {
            _busStopService = busStopService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBusStops([FromQuery] BusStopQueryFilter filter)
        {
            var result = await _busStopService.GetBusStopsAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusStopById(string id)
        {
            var result = await _busStopService.GetBusStopByIdAsync(id);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusStop([FromBody] CreateBusStopRequest request)
        {
            var result = await _busStopService.CreateBusStopAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetBusStopById), new { id = result.Data!.StopId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusStop(string id, [FromBody] UpdateBusStopRequest request)
        {
            var result = await _busStopService.UpdateBusStopAsync(id, request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusStop(string id)
        {
            var result = await _busStopService.DeleteBusStopAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
