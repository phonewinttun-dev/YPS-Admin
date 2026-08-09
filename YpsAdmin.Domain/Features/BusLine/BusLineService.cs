using Microsoft.EntityFrameworkCore;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.BusLine;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.BusLine
{
    public class BusLineService : IBusLineService
    {
        private readonly AppDbContext _context;

        public BusLineService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<BusLineDto>> GetBusLinesAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TblBusLines.AsNoTracking();
            int totalCount = await query.CountAsync(cancellationToken);
            int skip = (request.PageNumber - 1) * request.PageSize;
            var items = await query
                .OrderBy(b => b.BusNumber.Length)
                .ThenBy(b => EF.Functions.Unaccent(b.BusNumber))
                .Skip(skip)
                .Take(request.PageSize)
                .Select(b => new BusLineDto
                {
                    RouteId = b.RouteId,
                    BusNumber = b.BusNumber,
                    OutboundTitleMm = b.OutboundTitleMm,
                    OutboundTitleEn = b.OutboundTitleEn,
                    ReturnTitleMm = b.ReturnTitleMm,
                    ReturnTitleEn = b.ReturnTitleEn,
                    IsYpsAccepted = b.IsYpsAccepted ?? false
                })
                .ToListAsync(cancellationToken);
            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusLineDto>.Success(items, pagination, "Bus lines retrieved successfully.");
        }

        public async Task<Result<BusLineDto>> GetBusLineByIdAsync(int routeId, CancellationToken cancellationToken = default)
        {
            var busLine = await _context.TblBusLines
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.RouteId == routeId, cancellationToken);

            if (busLine == null)
            {
                return Result<BusLineDto>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            var dto = new BusLineDto
            {
                RouteId = busLine.RouteId,
                BusNumber = busLine.BusNumber,
                OutboundTitleMm = busLine.OutboundTitleMm,
                OutboundTitleEn = busLine.OutboundTitleEn,
                ReturnTitleMm = busLine.ReturnTitleMm,
                ReturnTitleEn = busLine.ReturnTitleEn,
                IsYpsAccepted = busLine.IsYpsAccepted ?? false
            };

            return Result<BusLineDto>.Success(dto, "Bus line retrieved successfully.");
        }

        public async Task<Result<BusLineDto>> CreateBusLineAsync(CreateBusLineRequest request, CancellationToken cancellationToken = default)
        {
            // Check for duplicate Route ID
            bool exists = await _context.TblBusLines.AnyAsync(b => b.RouteId == request.RouteId, cancellationToken);
            if (exists)
            {
                return Result<BusLineDto>.Failure($"A bus line with Route ID '{request.RouteId}' already exists.");
            }

            var busLine = new TblBusLine
            {
                RouteId = request.RouteId,
                BusNumber = request.BusNumber,
                OutboundTitleMm = request.OutboundTitleMm?.Trim(),
                OutboundTitleEn = request.OutboundTitleEn?.Trim(),
                ReturnTitleMm = request.ReturnTitleMm?.Trim(),
                ReturnTitleEn = request.ReturnTitleEn?.Trim(),
                IsYpsAccepted = request.IsYpsAccepted
            };

            _context.TblBusLines.Add(busLine);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new BusLineDto
            {
                RouteId = busLine.RouteId,
                BusNumber = busLine.BusNumber,
                OutboundTitleMm = busLine.OutboundTitleMm,
                OutboundTitleEn = busLine.OutboundTitleEn,
                ReturnTitleMm = busLine.ReturnTitleMm,
                ReturnTitleEn = busLine.ReturnTitleEn,
                IsYpsAccepted = busLine.IsYpsAccepted ?? false
            };

            return Result<BusLineDto>.Success(dto, "Bus line created successfully.");
        }

        public async Task<Result<BusLineDto>> UpdateBusLineAsync(int routeId, UpdateBusLineRequest request, CancellationToken cancellationToken = default)
        {
            var busLine = await _context.TblBusLines.FirstOrDefaultAsync(b => b.RouteId == routeId, cancellationToken);
            if (busLine == null)
            {
                return Result<BusLineDto>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            busLine.BusNumber = request.BusNumber;
            busLine.OutboundTitleMm = request.OutboundTitleMm?.Trim();
            busLine.OutboundTitleEn = request.OutboundTitleEn?.Trim();
            busLine.ReturnTitleMm = request.ReturnTitleMm?.Trim();
            busLine.ReturnTitleEn = request.ReturnTitleEn?.Trim();
            busLine.IsYpsAccepted = request.IsYpsAccepted;

            await _context.SaveChangesAsync(cancellationToken);

            var dto = new BusLineDto
            {
                RouteId = busLine.RouteId,
                BusNumber = busLine.BusNumber,
                OutboundTitleMm = busLine.OutboundTitleMm,
                OutboundTitleEn = busLine.OutboundTitleEn,
                ReturnTitleMm = busLine.ReturnTitleMm,
                ReturnTitleEn = busLine.ReturnTitleEn,
                IsYpsAccepted = busLine.IsYpsAccepted ?? false
            };

            return Result<BusLineDto>.Success(dto, "Bus line updated successfully.");
        }

        public async Task<Result<bool>> DeleteBusLineAsync(int routeId, CancellationToken cancellationToken = default)
        {
            var busLine = await _context.TblBusLines.FirstOrDefaultAsync(b => b.RouteId == routeId, cancellationToken);
            if (busLine == null)
            {
                return Result<bool>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            _context.TblBusLines.Remove(busLine);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Bus line deleted successfully.");
        }
    }
}
