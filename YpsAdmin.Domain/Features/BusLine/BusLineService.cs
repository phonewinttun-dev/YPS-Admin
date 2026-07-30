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

        //public async Task<PagedResult<BusLineDto>> GetBusLinesAsync(BusLineQueryFilter filter)
        //{
        //    var query = _context.Tblbuslines.AsNoTracking().AsQueryable();

        //    // Search by bus number if search term is provided
        //    if (!string.IsNullOrWhiteSpace(filter.SearchBusNumber))
        //    {
        //        string search = filter.SearchBusNumber.Trim().ToLower();
        //        query = query.Where(b => b.BusNumber.ToLower().Contains(search));
        //    }

        //    int totalCount = await query.CountAsync();

        //    int skip = (filter.PageNumber - 1) * filter.PageSize;

        //    var items = await query
        //        .OrderBy(b => b.BusNumber)
        //        .Skip(skip)
        //        .Take(filter.PageSize)
        //        .Select(b => new BusLineDto
        //        {
        //            RouteId = b.RouteId,
        //            BusNumber = b.BusNumber,
        //            OutboundTitleMm = b.OutboundTitleMm,
        //            OutboundTitleEn = b.OutboundTitleEn,
        //            ReturnTitleMm = b.ReturnTitleMm,
        //            ReturnTitleEn = b.ReturnTitleEn,
        //            IsYpsAccepted = b.IsYpsAccepted ?? false
        //        })
        //        .ToListAsync();

        //    var pagination = new Pagination(filter.PageNumber, filter.PageSize, totalCount);
        //    return PagedResult<BusLineDto>.Success(items, pagination, "Bus lines retrieved successfully.");
        //}

        public async Task<PagedResult<BusLineDto>> GetBusLinesAsync(PaginationRequest request)
        {
            var query = _context.Tblbuslines.AsNoTracking();
            int totalCount = await query.CountAsync();
            int skip = (request.PageNumber - 1) * request.PageSize;
            var items = await query
                .OrderBy(b => b.BusNumber)
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
                .ToListAsync();
            var pagination = new Pagination(request.PageNumber, request.PageSize, totalCount);
            return PagedResult<BusLineDto>.Success(items, pagination, "Bus lines retrieved successfully.");
        }

        public async Task<Result<BusLineDto>> GetBusLineByIdAsync(string routeId)
        {
            var busLine = await _context.Tblbuslines
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.RouteId == routeId);

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

        public async Task<Result<BusLineDto>> CreateBusLineAsync(CreateBusLineRequest request)
        {
            // Validate Route ID
            if (string.IsNullOrWhiteSpace(request.RouteId))
            {
                return Result<BusLineDto>.Failure("Route ID is required.");
            }

            // Check for duplicate Route ID
            bool exists = await _context.Tblbuslines.AnyAsync(b => b.RouteId == request.RouteId);
            if (exists)
            {
                return Result<BusLineDto>.Failure($"A bus line with Route ID '{request.RouteId}' already exists.");
            }

            var busLine = new Tblbusline
            {
                RouteId = request.RouteId.Trim(),
                BusNumber = request.BusNumber?.Trim() ?? string.Empty,
                OutboundTitleMm = request.OutboundTitleMm?.Trim(),
                OutboundTitleEn = request.OutboundTitleEn?.Trim(),
                ReturnTitleMm = request.ReturnTitleMm?.Trim(),
                ReturnTitleEn = request.ReturnTitleEn?.Trim(),
                IsYpsAccepted = request.IsYpsAccepted
            };

            _context.Tblbuslines.Add(busLine);
            await _context.SaveChangesAsync();

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

        public async Task<Result<BusLineDto>> UpdateBusLineAsync(string routeId, UpdateBusLineRequest request)
        {
            var busLine = await _context.Tblbuslines.FirstOrDefaultAsync(b => b.RouteId == routeId);
            if (busLine == null)
            {
                return Result<BusLineDto>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            busLine.BusNumber = request.BusNumber?.Trim() ?? busLine.BusNumber;
            busLine.OutboundTitleMm = request.OutboundTitleMm?.Trim();
            busLine.OutboundTitleEn = request.OutboundTitleEn?.Trim();
            busLine.ReturnTitleMm = request.ReturnTitleMm?.Trim();
            busLine.ReturnTitleEn = request.ReturnTitleEn?.Trim();
            busLine.IsYpsAccepted = request.IsYpsAccepted;

            await _context.SaveChangesAsync();

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

        public async Task<Result<bool>> DeleteBusLineAsync(string routeId)
        {
            var busLine = await _context.Tblbuslines.FirstOrDefaultAsync(b => b.RouteId == routeId);
            if (busLine == null)
            {
                return Result<bool>.Failure($"Bus line with Route ID '{routeId}' was not found.");
            }

            _context.Tblbuslines.Remove(busLine);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "Bus line deleted successfully.");
        }
    }
}
