using YpsAdmin.Domain.DTOs.BusLine;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusLine
{
    public interface IBusLineService
    {
        Task<PagedResult<BusLineDto>> GetBusLinesAsync(PaginationRequest request);
        Task<Result<BusLineDto>> GetBusLineByIdAsync(int routeId);
        Task<Result<BusLineDto>> CreateBusLineAsync(CreateBusLineRequest request);
        Task<Result<BusLineDto>> UpdateBusLineAsync(int routeId, UpdateBusLineRequest request);
        Task<Result<bool>> DeleteBusLineAsync(int routeId);
    }
}
