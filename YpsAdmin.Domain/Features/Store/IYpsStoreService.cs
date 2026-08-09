using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public interface IYpsStoreService
    {
        Task<PagedResult<YpsStoreDto>> GetYpsStoresAsync(YpsStoreGetRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<YpsStoreDto>> SearchYpsStoresAsync(YpsStoreSearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<YpsStoreDto>> GetYpsStoreByIdAsync(int storeId, CancellationToken cancellationToken = default);
        Task<Result<YpsStoreDto>> CreateYpsStoreAsync(CreateYpsStoreRequest request, CancellationToken cancellationToken = default);
        Task<Result<YpsStoreDto>> UpdateYpsStoreAsync(int storeId, UpdateYpsStoreRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteYpsStoreAsync(int storeId, CancellationToken cancellationToken = default);
        Task<Result<bool>> AssignNearestStopsAsync(int storeId, AssignNearestStopsRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> AssignServingBusLinesAsync(int storeId, AssignServingBusLinesRequest request, CancellationToken cancellationToken = default);
    }
}
