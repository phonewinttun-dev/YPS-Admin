using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.BusRoute;
using YpsAdmin.Domain.Features.BusRoute;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/bus-routes")]
    [Route("api/route-stops")]
    [ApiController]
    public class BusRoutesController : ControllerBase
    {
        private readonly IBusRouteService _busRouteService;

        public BusRoutesController(IBusRouteService busRouteService)
        {
            _busRouteService = busRouteService;
        }

        [HttpGet("bus/{busId}")]
        [HttpGet("bus-line/{busId}")]
        public async Task<IActionResult> GetFullRoute(long busId, CancellationToken cancellationToken)
        {
            var result = await _busRouteService.GetFullRouteAsync(busId, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignStops([FromBody] AssignBusRoutesRequest request, CancellationToken cancellationToken)
        {
            var result = await _busRouteService.AssignStopsToRouteAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderStops([FromBody] ReorderBusRoutesRequest request, CancellationToken cancellationToken)
        {
            var result = await _busRouteService.ReorderRouteStopsAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("bus/{busId}/stop/{stopOrder}")]
        public async Task<IActionResult> RemoveRouteStop(long busId, int stopOrder, CancellationToken cancellationToken)
        {
            var result = await _busRouteService.RemoveRouteStopAsync(busId, stopOrder, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
