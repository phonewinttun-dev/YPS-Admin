using YpsAdmin.Domain.DTOs.BusLine;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusLine
{
    public interface IBusLineService
    {
        Task<PagedResult<BusLineDto>> GetBusLinesAsync(BusLineQueryFilter filter);
        Task<Result<BusLineDto>> GetBusLineByIdAsync(string routeId);
        Task<Result<BusLineDto>> CreateBusLineAsync(CreateBusLineRequest request);
        Task<Result<BusLineDto>> UpdateBusLineAsync(string routeId, UpdateBusLineRequest request);
        Task<Result<bool>> DeleteBusLineAsync(string routeId);
    }
}
