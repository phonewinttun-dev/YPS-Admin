using System.Threading;
using System.Threading.Tasks;
using YpsAdmin.Domain.DTOs.Bus;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Bus
{
    public interface IBusService
    {
        Task<PagedResult<BusDto>> GetBusesAsync(BusGetRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<BusDto>> SearchBusesAsync(BusSearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusDto>> GetBusByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<Result<BusDto>> CreateBusAsync(CreateBusRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusDto>> UpdateBusAsync(long id, UpdateBusRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteBusAsync(long id, CancellationToken cancellationToken = default);
    }
}
