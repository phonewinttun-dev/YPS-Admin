using YpsAdmin.Domain.DTOs.RouteStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.RouteStop
{
    public interface IRouteStopService
    {
        Task<Result<FullRouteResponseDto>> GetFullRouteAsync(string routeId);
        Task<Result<bool>> AssignStopsToRouteAsync(AssignRouteStopsRequest request);
        Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request);
        Task<Result<bool>> RemoveRouteStopAsync(int routeStopId);
    }
}
