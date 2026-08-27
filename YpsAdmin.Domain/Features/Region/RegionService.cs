using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Region;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Region
{
    public class RegionService : IRegionService
    {
        private readonly AppDbContext _context;

        public RegionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RegionDto>> GetRegionsAsync(RegionGetRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblRegions.AsNoTracking().Where(r => r.DeleteFlag != true);

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await query
                .OrderBy(r => r.RegionName)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(r => new RegionDto
                {
                    Id = r.Id,
                    RegionName = r.RegionName,
                    DeleteFlag = r.DeleteFlag ?? false,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<RegionDto>.Success(items, pagination, "Regions retrieved successfully.");
        }

        public async Task<PagedResult<RegionDto>> SearchRegionsAsync(RegionSearchRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblRegions.AsNoTracking().Where(r => r.DeleteFlag != true);

            if (!string.IsNullOrWhiteSpace(request.RegionName))
            {
                string search = $"%{request.RegionName.Trim()}%";
                query = query.Where(r => EF.Functions.ILike(r.RegionName, search));
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await query
                .OrderBy(r => r.RegionName)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(r => new RegionDto
                {
                    Id = r.Id,
                    RegionName = r.RegionName,
                    DeleteFlag = r.DeleteFlag ?? false,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<RegionDto>.Success(items, pagination, "Region search completed successfully.");
        }

        public async Task<Result<RegionDto>> GetRegionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var region = await _context.TblRegions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.DeleteFlag != true, cancellationToken);

            if (region == null)
            {
                return Result<RegionDto>.Failure($"Region with ID '{id}' was not found.");
            }

            var dto = new RegionDto
            {
                Id = region.Id,
                RegionName = region.RegionName,
                DeleteFlag = region.DeleteFlag ?? false,
                CreatedAt = region.CreatedAt,
                UpdatedAt = region.UpdatedAt
            };

            return Result<RegionDto>.Success(dto, "Region retrieved successfully.");
        }

        public async Task<Result<RegionDto>> CreateRegionAsync(CreateRegionRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RegionName))
            {
                return Result<RegionDto>.Failure("Region name is required.");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var region = new TblRegion
            {
                RegionName = request.RegionName.Trim(),
                DeleteFlag = false,
                CreatedAt = today,
                UpdatedAt = today
            };

            _context.TblRegions.Add(region);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new RegionDto
            {
                Id = region.Id,
                RegionName = region.RegionName,
                DeleteFlag = false,
                CreatedAt = region.CreatedAt,
                UpdatedAt = region.UpdatedAt
            };

            return Result<RegionDto>.Success(dto, "Region created successfully.");
        }

        public async Task<Result<RegionDto>> UpdateRegionAsync(int id, UpdateRegionRequest request, CancellationToken cancellationToken = default)
        {
            var region = await _context.TblRegions.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (region == null)
            {
                return Result<RegionDto>.Failure($"Region with ID '{id}' was not found.");
            }

            region.RegionName = request.RegionName.Trim();
            if (request.DeleteFlag.HasValue)
            {
                region.DeleteFlag = request.DeleteFlag.Value;
            }
            region.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            var dto = new RegionDto
            {
                Id = region.Id,
                RegionName = region.RegionName,
                DeleteFlag = region.DeleteFlag ?? false,
                CreatedAt = region.CreatedAt,
                UpdatedAt = region.UpdatedAt
            };

            return Result<RegionDto>.Success(dto, "Region updated successfully.");
        }

        public async Task<Result<bool>> DeleteRegionAsync(int id, CancellationToken cancellationToken = default)
        {
            var region = await _context.TblRegions.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (region == null)
            {
                return Result<bool>.Failure($"Region with ID '{id}' was not found.");
            }

            region.DeleteFlag = true;
            region.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Region deleted successfully.");
        }
    }
}
