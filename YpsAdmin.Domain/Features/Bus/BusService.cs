using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Bus;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Bus
{
    public class BusService : IBusService
    {
        private readonly AppDbContext _context;

        public BusService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<BusDto>> GetBusesAsync(BusGetRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblBuses.AsNoTracking().Where(b => b.DeleteFlag != true);

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await query
                .OrderBy(b => b.BusNumber)
                .ThenBy(b => b.VariantId)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(b => new BusDto
                {
                    Id = b.Id,
                    BusNumber = b.BusNumber,
                    VariantId = b.VariantId,
                    IsCardAccepted = b.IsCardAccepted ?? false,
                    IsReversed = b.IsReversed ?? false,
                    DeleteFlag = b.DeleteFlag ?? false,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusDto>.Success(items, pagination, "Buses retrieved successfully.");
        }

        public async Task<PagedResult<BusDto>> SearchBusesAsync(BusSearchRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblBuses.AsNoTracking().Where(b => b.DeleteFlag != true);

            if (!string.IsNullOrWhiteSpace(request.BusNumber))
            {
                if (long.TryParse(request.BusNumber.Trim(), out long busNum))
                {
                    query = query.Where(b => b.BusNumber == busNum);
                }
                else
                {
                    string search = $"%{request.BusNumber.Trim()}%";
                    query = query.Where(b => b.VariantId != null && EF.Functions.ILike(b.VariantId, search));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.VariantId))
            {
                string searchVariant = $"%{request.VariantId.Trim()}%";
                query = query.Where(b => b.VariantId != null && EF.Functions.ILike(b.VariantId, searchVariant));
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;

            var items = await query
                .OrderBy(b => b.BusNumber)
                .ThenBy(b => b.VariantId)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(b => new BusDto
                {
                    Id = b.Id,
                    BusNumber = b.BusNumber,
                    VariantId = b.VariantId,
                    IsCardAccepted = b.IsCardAccepted ?? false,
                    IsReversed = b.IsReversed ?? false,
                    DeleteFlag = b.DeleteFlag ?? false,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusDto>.Success(items, pagination, "Bus search completed successfully.");
        }

        public async Task<Result<BusDto>> GetBusByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var bus = await _context.TblBuses
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.DeleteFlag != true, cancellationToken);

            if (bus == null)
            {
                return Result<BusDto>.Failure($"Bus with ID '{id}' was not found.");
            }

            var dto = new BusDto
            {
                Id = bus.Id,
                BusNumber = bus.BusNumber,
                VariantId = bus.VariantId,
                IsCardAccepted = bus.IsCardAccepted ?? false,
                IsReversed = bus.IsReversed ?? false,
                DeleteFlag = bus.DeleteFlag ?? false,
                CreatedAt = bus.CreatedAt,
                UpdatedAt = bus.UpdatedAt
            };

            return Result<BusDto>.Success(dto, "Bus retrieved successfully.");
        }

        public async Task<Result<BusDto>> CreateBusAsync(CreateBusRequest request, CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var bus = new TblBus
            {
                BusNumber = request.BusNumber,
                VariantId = request.VariantId?.Trim(),
                IsCardAccepted = request.IsCardAccepted,
                IsReversed = request.IsReversed,
                DeleteFlag = false,
                CreatedAt = today,
                UpdatedAt = today
            };

            _context.TblBuses.Add(bus);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new BusDto
            {
                Id = bus.Id,
                BusNumber = bus.BusNumber,
                VariantId = bus.VariantId,
                IsCardAccepted = bus.IsCardAccepted ?? false,
                IsReversed = bus.IsReversed ?? false,
                DeleteFlag = false,
                CreatedAt = bus.CreatedAt,
                UpdatedAt = bus.UpdatedAt
            };

            return Result<BusDto>.Success(dto, "Bus created successfully.");
        }

        public async Task<Result<BusDto>> UpdateBusAsync(long id, UpdateBusRequest request, CancellationToken cancellationToken = default)
        {
            var bus = await _context.TblBuses.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            if (bus == null)
            {
                return Result<BusDto>.Failure($"Bus with ID '{id}' was not found.");
            }

            bus.BusNumber = request.BusNumber;
            bus.VariantId = request.VariantId?.Trim();
            bus.IsCardAccepted = request.IsCardAccepted;
            bus.IsReversed = request.IsReversed;
            if (request.DeleteFlag.HasValue)
            {
                bus.DeleteFlag = request.DeleteFlag.Value;
            }
            bus.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            var dto = new BusDto
            {
                Id = bus.Id,
                BusNumber = bus.BusNumber,
                VariantId = bus.VariantId,
                IsCardAccepted = bus.IsCardAccepted ?? false,
                IsReversed = bus.IsReversed ?? false,
                DeleteFlag = bus.DeleteFlag ?? false,
                CreatedAt = bus.CreatedAt,
                UpdatedAt = bus.UpdatedAt
            };

            return Result<BusDto>.Success(dto, "Bus updated successfully.");
        }

        public async Task<Result<bool>> DeleteBusAsync(long id, CancellationToken cancellationToken = default)
        {
            var bus = await _context.TblBuses.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            if (bus == null)
            {
                return Result<bool>.Failure($"Bus with ID '{id}' was not found.");
            }

            bus.DeleteFlag = true;
            bus.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Bus deleted successfully.");
        }
    }
}
