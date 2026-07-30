using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public interface IYpsStoreService
    {
        Task<PagedResult<YpsStoreDto>> GetYpsStoresAsync(YpsStoreQueryFilter filter);
        Task<Result<YpsStoreDto>> GetYpsStoreByIdAsync(int storeId);
        Task<Result<YpsStoreDto>> CreateYpsStoreAsync(CreateYpsStoreRequest request);
        Task<Result<YpsStoreDto>> UpdateYpsStoreAsync(int storeId, UpdateYpsStoreRequest request);
        Task<Result<bool>> DeleteYpsStoreAsync(int storeId);
        Task<Result<bool>> AssignNearestStopsAsync(int storeId, AssignNearestStopsRequest request);
        Task<Result<bool>> AssignServingBusLinesAsync(int storeId, AssignServingBusLinesRequest request);
    }
}
