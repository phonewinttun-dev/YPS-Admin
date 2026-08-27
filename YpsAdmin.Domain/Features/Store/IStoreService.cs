using System.Threading;
using System.Threading.Tasks;
using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public interface IStoreService
    {
        Task<PagedResult<StoreDto>> GetStoresAsync(StoreGetRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<StoreDto>> SearchStoresAsync(StoreSearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<StoreDto>> GetStoreByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<StoreDto>> CreateStoreAsync(CreateStoreRequest request, CancellationToken cancellationToken = default);
        Task<Result<StoreDto>> UpdateStoreAsync(int id, UpdateStoreRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteStoreAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<bool>> AssignNearestStopsAsync(int storeId, AssignNearestStopsRequest request, CancellationToken cancellationToken = default);
    }
}
