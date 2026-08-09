using YpsAdmin.Domain.DTOs.Township;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Township
{
    public interface ITownshipService
    {
        Task<PagedResult<TownshipDto>> GetTownshipsAsync(TownshipGetRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<TownshipDto>> SearchTownshipsAsync(TownshipSearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<TownshipDto>> GetTownshipByIdAsync(int townshipId, CancellationToken cancellationToken = default);
        Task<Result<TownshipDto>> CreateTownshipAsync(CreateTownshipRequest request, CancellationToken cancellationToken = default);
        Task<Result<TownshipDto>> UpdateTownshipAsync(int townshipId, UpdateTownshipRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteTownshipAsync(int townshipId, CancellationToken cancellationToken = default);
    }
}
