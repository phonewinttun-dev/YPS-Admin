using System.Threading;
using System.Threading.Tasks;
using YpsAdmin.Domain.DTOs.Region;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Region
{
    public interface IRegionService
    {
        Task<PagedResult<RegionDto>> GetRegionsAsync(RegionGetRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<RegionDto>> SearchRegionsAsync(RegionSearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<RegionDto>> GetRegionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<RegionDto>> CreateRegionAsync(CreateRegionRequest request, CancellationToken cancellationToken = default);
        Task<Result<RegionDto>> UpdateRegionAsync(int id, UpdateRegionRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteRegionAsync(int id, CancellationToken cancellationToken = default);
    }
}
