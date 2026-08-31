using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YpsAdmin.Domain.DTOs.Dashboard;
using YpsAdmin.Domain.Features.Dashboard;
using YpsAdmin.Shared;

namespace YpsAdmin.Api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        [HttpGet]
        [ProducesResponseType(typeof(Result<DashboardSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetDashboardSummaryAsync(cancellationToken);
            return Ok(result);
        }
    }
}
