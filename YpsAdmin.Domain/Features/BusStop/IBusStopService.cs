using YpsAdmin.Domain.DTOs.BusStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusStop
{
    public interface IBusStopService
    {
        Task<PagedResult<BusStopDto>> GetBusStopsAsync(BusStopQueryFilter filter, CancellationToken cancellationToken = default);
        Task<Result<BusStopDto>> GetBusStopByIdAsync(int stopId, CancellationToken cancellationToken = default);
        Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusStopDto>> UpdateBusStopAsync(int stopId, UpdateBusStopRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteBusStopAsync(int stopId, CancellationToken cancellationToken = default);
    }
}
