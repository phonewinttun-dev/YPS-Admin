using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Township;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Township
{
    public class TownshipService : ITownshipService
    {
        private readonly AppDbContext _context;

        public TownshipService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TownshipDto>> GetTownshipsAsync(TownshipQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _context.TblTownships.AsNoTracking().Where(t => t.DeleteFlag != true);

            if (!string.IsNullOrWhiteSpace(filter.SearchName))
            {
                string search = $"%{filter.SearchName.Trim()}%";
                query = query.Where(t =>
                    EF.Functions.ILike(t.TownshipNameMm, search) ||
                    (t.TownshipNameEn != null && EF.Functions.ILike(t.TownshipNameEn, search)));
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(t => t.TownshipNameMm)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(t => new TownshipDto
                {
                    TownshipId = t.TownshipId,
                    TownshipNameMm = t.TownshipNameMm,
                    TownshipNameEn = t.TownshipNameEn,
                    DeleteFlag = t.DeleteFlag ?? false
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(filter.PageNumber, filter.PageSize, totalCount);
            return PagedResult<TownshipDto>.Success(items, pagination, "Townships retrieved successfully.");
        }

        public async Task<Result<TownshipDto>> GetTownshipByIdAsync(int townshipId, CancellationToken cancellationToken = default)
        {
            var township = await _context.TblTownships
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TownshipId == townshipId && t.DeleteFlag != true, cancellationToken);

            if (township == null)
            {
                return Result<TownshipDto>.Failure($"Township with ID '{townshipId}' was not found.");
            }

            var dto = new TownshipDto
            {
                TownshipId = township.TownshipId,
                TownshipNameMm = township.TownshipNameMm,
                TownshipNameEn = township.TownshipNameEn,
                DeleteFlag = township.DeleteFlag ?? false
            };

            return Result<TownshipDto>.Success(dto, "Township retrieved successfully.");
        }

        public async Task<Result<TownshipDto>> CreateTownshipAsync(CreateTownshipRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.TownshipNameMm))
            {
                return Result<TownshipDto>.Failure("Township name in Myanmar is required.");
            }

            var township = new TblTownship
            {
                TownshipNameMm = request.TownshipNameMm.Trim(),
                TownshipNameEn = request.TownshipNameEn?.Trim(),
                DeleteFlag = false
            };

            _context.TblTownships.Add(township);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new TownshipDto
            {
                TownshipId = township.TownshipId,
                TownshipNameMm = township.TownshipNameMm,
                TownshipNameEn = township.TownshipNameEn,
                DeleteFlag = false
            };

            return Result<TownshipDto>.Success(dto, "Township created successfully.");
        }

        public async Task<Result<TownshipDto>> UpdateTownshipAsync(int townshipId, UpdateTownshipRequest request, CancellationToken cancellationToken = default)
        {
            var township = await _context.TblTownships.FirstOrDefaultAsync(t => t.TownshipId == townshipId, cancellationToken);
            if (township == null)
            {
                return Result<TownshipDto>.Failure($"Township with ID '{townshipId}' was not found.");
            }

            township.TownshipNameMm = request.TownshipNameMm?.Trim() ?? township.TownshipNameMm;
            township.TownshipNameEn = request.TownshipNameEn?.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            var dto = new TownshipDto
            {
                TownshipId = township.TownshipId,
                TownshipNameMm = township.TownshipNameMm,
                TownshipNameEn = township.TownshipNameEn,
                DeleteFlag = township.DeleteFlag ?? false
            };

            return Result<TownshipDto>.Success(dto, "Township updated successfully.");
        }

        public async Task<Result<bool>> DeleteTownshipAsync(int townshipId, CancellationToken cancellationToken = default)
        {
            var township = await _context.TblTownships.FirstOrDefaultAsync(t => t.TownshipId == townshipId, cancellationToken);
            if (township == null)
            {
                return Result<bool>.Failure($"Township with ID '{townshipId}' was not found.");
            }

            township.DeleteFlag = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Township deleted successfully.");
        }
    }
}
