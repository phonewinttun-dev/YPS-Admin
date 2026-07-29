using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public interface IYpsStoreService
    {
        Task<PagedResult<YpsStoreDto>> GetYpsStoresAsync(YpsStoreQueryFilter filter);
        Task<Result<YpsStoreDto>> GetYpsStoreByIdAsync(string storeId);
        Task<Result<YpsStoreDto>> CreateYpsStoreAsync(CreateYpsStoreRequest request);
        Task<Result<YpsStoreDto>> UpdateYpsStoreAsync(string storeId, UpdateYpsStoreRequest request);
        Task<Result<bool>> DeleteYpsStoreAsync(string storeId);
        Task<Result<bool>> AssignNearestStopsAsync(string storeId, AssignNearestStopsRequest request);
        Task<Result<bool>> AssignServingBusLinesAsync(string storeId, AssignServingBusLinesRequest request);
    }
}
