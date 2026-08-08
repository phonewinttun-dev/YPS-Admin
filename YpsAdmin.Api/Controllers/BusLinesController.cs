using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.BusLine;
using YpsAdmin.Domain.Features.BusLine;
using YpsAdmin.Shared;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/bus-lines")]
    [ApiController]
    public class BusLinesController : ControllerBase
    {
        private readonly IBusLineService _busLineService;

        public BusLinesController(IBusLineService busLineService)
        {
            _busLineService = busLineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBusLines([FromQuery] PaginationRequest filter, CancellationToken cancellationToken)
        {
            var result = await _busLineService.GetBusLinesAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusLineById(int id, CancellationToken cancellationToken)
        {
            var result = await _busLineService.GetBusLineByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusLine([FromBody] CreateBusLineRequest request, CancellationToken cancellationToken)
        {
            var result = await _busLineService.CreateBusLineAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetBusLineById), new { id = result.Data!.RouteId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusLine(int id, [FromBody] UpdateBusLineRequest request, CancellationToken cancellationToken)
        {
            var result = await _busLineService.UpdateBusLineAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusLine(int id, CancellationToken cancellationToken)
        {
            var result = await _busLineService.DeleteBusLineAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
