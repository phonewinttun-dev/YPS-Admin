using System.Threading;
using System.Threading.Tasks;
using YpsAdmin.Domain.DTOs.BusRoute;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusRoute
{
    public interface IBusRouteService
    {
        Task<Result<FullRouteResponseDto>> GetFullRouteAsync(long busId, CancellationToken cancellationToken = default);
        Task<Result<bool>> AssignStopsToRouteAsync(AssignBusRoutesRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> ReorderRouteStopsAsync(ReorderBusRoutesRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> RemoveRouteStopAsync(long busId, int stopOrder, CancellationToken cancellationToken = default);
    }
}
