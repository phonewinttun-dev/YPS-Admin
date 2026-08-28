using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Domain.Features.Store;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/stores")]
    [Route("api/yps-stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStores([FromQuery] StoreGetRequest request, CancellationToken cancellationToken)
        {
            var result = await _storeService.GetStoresAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStores([FromQuery] StoreSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _storeService.SearchStoresAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id, CancellationToken cancellationToken)
        {
            var result = await _storeService.GetStoreByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request, CancellationToken cancellationToken)
        {
            var result = await _storeService.CreateStoreAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetStoreById), new { id = result.Data!.Id }, result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreRequest request, CancellationToken cancellationToken)
        {
            var result = await _storeService.UpdateStoreAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id, CancellationToken cancellationToken)
        {
            var result = await _storeService.DeleteStoreAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("{id}/nearest-stops")]
        public async Task<IActionResult> AssignNearestStops(int id, [FromBody] AssignNearestStopsRequest request, CancellationToken cancellationToken)
        {
            var result = await _storeService.AssignNearestStopsAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
