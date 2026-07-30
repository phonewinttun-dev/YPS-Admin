using YpsAdmin.Domain.DTOs.BusStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusStop
{
    public interface IBusStopService
    {
        Task<PagedResult<BusStopDto>> GetBusStopsAsync(BusStopQueryFilter filter);
        Task<Result<BusStopDto>> GetBusStopByIdAsync(int stopId);
        Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request);
        Task<Result<BusStopDto>> UpdateBusStopAsync(int stopId, UpdateBusStopRequest request);
        Task<Result<bool>> DeleteBusStopAsync(int stopId);
    }
}
