using YpsAdmin.Domain.DTOs.Township;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Township
{
    public interface ITownshipService
    {
        Task<PagedResult<TownshipDto>> GetTownshipsAsync(TownshipQueryFilter filter);
        Task<Result<TownshipDto>> GetTownshipByIdAsync(int townshipId);
        Task<Result<TownshipDto>> CreateTownshipAsync(CreateTownshipRequest request);
        Task<Result<TownshipDto>> UpdateTownshipAsync(int townshipId, UpdateTownshipRequest request);
        Task<Result<bool>> DeleteTownshipAsync(int townshipId);
    }
}
