using System.Threading;
using System.Threading.Tasks;
using YpsAdmin.Domain.DTOs.Dashboard;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Dashboard
{
    public interface IDashboardService
    {
        Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    }
}
