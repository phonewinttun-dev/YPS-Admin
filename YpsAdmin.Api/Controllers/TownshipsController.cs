using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.Township;
using YpsAdmin.Domain.Features.Township;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/townships")]
    [ApiController]
    public class TownshipsController : ControllerBase
    {
        private readonly ITownshipService _townshipService;

        public TownshipsController(ITownshipService townshipService)
        {
            _townshipService = townshipService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTownships([FromQuery] TownshipGetRequest request, CancellationToken cancellationToken)
        {
            var result = await _townshipService.GetTownshipsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTownships([FromQuery] TownshipSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _townshipService.SearchTownshipsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTownshipById(int id, CancellationToken cancellationToken)
        {
            var result = await _townshipService.GetTownshipByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTownship([FromBody] CreateTownshipRequest request, CancellationToken cancellationToken)
        {
            var result = await _townshipService.CreateTownshipAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetTownshipById), new { id = result.Data!.TownshipId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTownship(int id, [FromBody] UpdateTownshipRequest request, CancellationToken cancellationToken)
        {
            var result = await _townshipService.UpdateTownshipAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTownship(int id, CancellationToken cancellationToken)
        {
            var result = await _townshipService.DeleteTownshipAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
