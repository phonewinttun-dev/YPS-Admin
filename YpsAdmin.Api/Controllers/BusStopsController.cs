using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.BusStop;
using YpsAdmin.Domain.Features.BusStop;

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
        public async Task<IActionResult> GetBusStops([FromQuery] BusStopGetRequest request, CancellationToken cancellationToken)
        {
            var result = await _busStopService.GetBusStopsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBusStops([FromQuery] BusStopSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _busStopService.SearchBusStopsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusStopById(long id, CancellationToken cancellationToken)
        {
            var result = await _busStopService.GetBusStopByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusStop([FromBody] CreateBusStopRequest request, CancellationToken cancellationToken)
        {
            var result = await _busStopService.CreateBusStopAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetBusStopById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusStop(long id, [FromBody] UpdateBusStopRequest request, CancellationToken cancellationToken)
        {
            var result = await _busStopService.UpdateBusStopAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusStop(long id, CancellationToken cancellationToken)
        {
            var result = await _busStopService.DeleteBusStopAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
