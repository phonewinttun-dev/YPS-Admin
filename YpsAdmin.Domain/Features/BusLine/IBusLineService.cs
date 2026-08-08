using YpsAdmin.Domain.DTOs.BusLine;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusLine
{
    public interface IBusLineService
    {
        Task<PagedResult<BusLineDto>> GetBusLinesAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusLineDto>> GetBusLineByIdAsync(int routeId, CancellationToken cancellationToken = default);
        Task<Result<BusLineDto>> CreateBusLineAsync(CreateBusLineRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusLineDto>> UpdateBusLineAsync(int routeId, UpdateBusLineRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteBusLineAsync(int routeId, CancellationToken cancellationToken = default);
    }
}
