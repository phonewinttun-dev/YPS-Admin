using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.Region;
using YpsAdmin.Domain.Features.Region;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/regions")]
    [Route("api/townships")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRegions([FromQuery] RegionGetRequest request, CancellationToken cancellationToken)
        {
            var result = await _regionService.GetRegionsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRegions([FromQuery] RegionSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _regionService.SearchRegionsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegionById(int id, CancellationToken cancellationToken)
        {
            var result = await _regionService.GetRegionByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegion([FromBody] CreateRegionRequest request, CancellationToken cancellationToken)
        {
            var result = await _regionService.CreateRegionAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetRegionById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateRegionRequest request, CancellationToken cancellationToken)
        {
            var result = await _regionService.UpdateRegionAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id, CancellationToken cancellationToken)
        {
            var result = await _regionService.DeleteRegionAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
