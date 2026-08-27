using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.Bus;
using YpsAdmin.Domain.Features.Bus;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/buses")]
    [Route("api/bus-lines")]
    [ApiController]
    public class BusesController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusesController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBuses([FromQuery] BusGetRequest request, CancellationToken cancellationToken)
        {
            var result = await _busService.GetBusesAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBuses([FromQuery] BusSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _busService.SearchBusesAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusById(long id, CancellationToken cancellationToken)
        {
            var result = await _busService.GetBusByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBus([FromBody] CreateBusRequest request, CancellationToken cancellationToken)
        {
            var result = await _busService.CreateBusAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetBusById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBus(long id, [FromBody] UpdateBusRequest request, CancellationToken cancellationToken)
        {
            var result = await _busService.UpdateBusAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(long id, CancellationToken cancellationToken)
        {
            var result = await _busService.DeleteBusAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
