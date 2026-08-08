using YpsAdmin.Domain.DTOs.RouteStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.RouteStop
{
    public interface IRouteStopService
    {
        Task<Result<FullRouteResponseDto>> GetFullRouteAsync(int routeId, CancellationToken cancellationToken = default);
        Task<Result<bool>> AssignStopsToRouteAsync(AssignRouteStopsRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> RemoveRouteStopAsync(int routeStopId, CancellationToken cancellationToken = default);
    }
}
