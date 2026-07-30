using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.RouteStop;
using YpsAdmin.Domain.Features.RouteStop;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/route-stops")]
    [ApiController]
    public class RouteStopsController : ControllerBase
    {
        private readonly IRouteStopService _routeStopService;

        public RouteStopsController(IRouteStopService routeStopService)
        {
            _routeStopService = routeStopService;
        }

        [HttpGet("bus-line/{busLineId}")]
        public async Task<IActionResult> GetFullRoute(int busLineId)
        {
            var result = await _routeStopService.GetFullRouteAsync(busLineId);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignStops([FromBody] AssignRouteStopsRequest request)
        {
            var result = await _routeStopService.AssignStopsToRouteAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderStops([FromBody] ReorderRouteStopsRequest request)
        {
            var result = await _routeStopService.ReorderRouteStopsAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{routeStopId}")]
        public async Task<IActionResult> RemoveRouteStop(int routeStopId)
        {
            var result = await _routeStopService.RemoveRouteStopAsync(routeStopId);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
