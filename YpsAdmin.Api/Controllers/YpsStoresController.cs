using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Domain.Features.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/yps-stores")]
    [ApiController]
    public class YpsStoresController : ControllerBase
    {
        private readonly IYpsStoreService _ypsStoreService;

        public YpsStoresController(IYpsStoreService ypsStoreService)
        {
            _ypsStoreService = ypsStoreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetYpsStores([FromQuery] YpsStoreQueryFilter filter)
        {
            var result = await _ypsStoreService.GetYpsStoresAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetYpsStoreById(string id)
        {
            var result = await _ypsStoreService.GetYpsStoreByIdAsync(id);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYpsStore([FromBody] CreateYpsStoreRequest request)
        {
            var result = await _ypsStoreService.CreateYpsStoreAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetYpsStoreById), new { id = result.Data!.StoreId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateYpsStore(string id, [FromBody] UpdateYpsStoreRequest request)
        {
            var result = await _ypsStoreService.UpdateYpsStoreAsync(id, request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteYpsStore(string id)
        {
            var result = await _ypsStoreService.DeleteYpsStoreAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("{id}/nearest-stops")]
        public async Task<IActionResult> AssignNearestStops(string id, [FromBody] AssignNearestStopsRequest request)
        {
            var result = await _ypsStoreService.AssignNearestStopsAsync(id, request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("{id}/serving-bus-lines")]
        public async Task<IActionResult> AssignServingBusLines(string id, [FromBody] AssignServingBusLinesRequest request)
        {
            var result = await _ypsStoreService.AssignServingBusLinesAsync(id, request);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
