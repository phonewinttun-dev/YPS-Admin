using System.Threading;
using System.Threading.Tasks;
using YpsAdmin.Domain.DTOs.BusStop;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusStop
{
    public interface IBusStopService
    {
        Task<PagedResult<BusStopDto>> GetBusStopsAsync(BusStopGetRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<BusStopDto>> SearchBusStopsAsync(BusStopSearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusStopDto>> GetBusStopByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<Result<BusStopDto>> CreateBusStopAsync(CreateBusStopRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusStopDto>> UpdateBusStopAsync(long id, UpdateBusStopRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteBusStopAsync(long id, CancellationToken cancellationToken = default);
    }
}
