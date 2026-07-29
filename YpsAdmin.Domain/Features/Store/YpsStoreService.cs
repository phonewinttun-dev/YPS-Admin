using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using YpsAdmin.Database.AppDbContextModels;
using YpsAdmin.Domain.DTOs.Store;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.Features.Store
{
    public class YpsStoreService : IYpsStoreService
    {
        private readonly AppDbContext _context;

        public YpsStoreService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<YpsStoreDto>> GetYpsStoresAsync(YpsStoreQueryFilter filter)
        {
            var query = _context.Tblypsstores.AsNoTracking().AsQueryable();

            // Search by store name (MM or EN) if search term is provided
            if (!string.IsNullOrWhiteSpace(filter.SearchName))
            {
                string search = filter.SearchName.Trim().ToLower();
                query = query.Where(s =>
                    s.NameMm.ToLower().Contains(search) ||
                    (s.NameEn != null && s.NameEn.ToLower().Contains(search)));
            }

            int totalCount = await query.CountAsync();

            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var stores = await query
                .Include(s => s.TblypsstoreNeareststops)
                .Include(s => s.TblypsstoreServingbuslines)
                .OrderBy(s => s.StoreId)
                .Skip(skip)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtos = stores.Select(s => new YpsStoreDto
            {
                StoreId = s.StoreId,
                NameMm = s.NameMm,
                NameEn = s.NameEn,
                Category = s.Category,
                TownshipMm = s.TownshipMm,
                TownshipEn = s.TownshipEn,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                NearestStops = s.TblypsstoreNeareststops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = s.TblypsstoreServingbuslines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            }).ToList();

            var pagination = new Pagination(filter.PageNumber, filter.PageSize, totalCount);
            return PagedResult<YpsStoreDto>.Success(dtos, pagination, "YPS stores retrieved successfully.");
        }

        public async Task<Result<YpsStoreDto>> GetYpsStoreByIdAsync(string storeId)
        {
            var store = await _context.Tblypsstores
                .AsNoTracking()
                .Include(s => s.TblypsstoreNeareststops)
                .Include(s => s.TblypsstoreServingbuslines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<YpsStoreDto>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipMm = store.TownshipMm,
                TownshipEn = store.TownshipEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                NearestStops = store.TblypsstoreNeareststops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = store.TblypsstoreServingbuslines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store retrieved successfully.");
        }

        public async Task<Result<YpsStoreDto>> CreateYpsStoreAsync(CreateYpsStoreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StoreId))
            {
                return Result<YpsStoreDto>.Failure("Store ID is required.");
            }

            bool exists = await _context.Tblypsstores.AnyAsync(s => s.StoreId == request.StoreId);
            if (exists)
            {
                return Result<YpsStoreDto>.Failure($"A YPS store with Store ID '{request.StoreId}' already exists.");
            }

            // Convert Latitude and Longitude to PostGIS Point geometry (WGS84 SRID 4326)
            Point? geom = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                geom = new Point((double)request.Longitude.Value, (double)request.Latitude.Value)
                {
                    SRID = 4326
                };
            }

            var store = new Tblypsstore
            {
                StoreId = request.StoreId.Trim(),
                NameMm = request.NameMm?.Trim() ?? string.Empty,
                NameEn = request.NameEn?.Trim(),
                Category = request.Category?.Trim(),
                TownshipMm = request.TownshipMm?.Trim(),
                TownshipEn = request.TownshipEn?.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Geom = geom
            };

            _context.Tblypsstores.Add(store);
            await _context.SaveChangesAsync();

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipMm = store.TownshipMm,
                TownshipEn = store.TownshipEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store created successfully.");
        }

        public async Task<Result<YpsStoreDto>> UpdateYpsStoreAsync(string storeId, UpdateYpsStoreRequest request)
        {
            var store = await _context.Tblypsstores
                .Include(s => s.TblypsstoreNeareststops)
                .Include(s => s.TblypsstoreServingbuslines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<YpsStoreDto>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            store.NameMm = request.NameMm?.Trim() ?? store.NameMm;
            store.NameEn = request.NameEn?.Trim();
            store.Category = request.Category?.Trim();
            store.TownshipMm = request.TownshipMm?.Trim();
            store.TownshipEn = request.TownshipEn?.Trim();
            store.Latitude = request.Latitude;
            store.Longitude = request.Longitude;

            // Update PostGIS Point geometry
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                store.Geom = new Point((double)request.Longitude.Value, (double)request.Latitude.Value)
                {
                    SRID = 4326
                };
            }
            else
            {
                store.Geom = null;
            }

            await _context.SaveChangesAsync();

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipMm = store.TownshipMm,
                TownshipEn = store.TownshipEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                NearestStops = store.TblypsstoreNeareststops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = store.TblypsstoreServingbuslines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store updated successfully.");
        }

        public async Task<Result<bool>> DeleteYpsStoreAsync(string storeId)
        {
            var store = await _context.Tblypsstores.FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            _context.Tblypsstores.Remove(store);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "YPS store deleted successfully.");
        }

        public async Task<Result<bool>> AssignNearestStopsAsync(string storeId, AssignNearestStopsRequest request)
        {
            var store = await _context.Tblypsstores
                .Include(s => s.TblypsstoreNeareststops)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            // Remove existing nearest stops
            _context.TblypsstoreNeareststops.RemoveRange(store.TblypsstoreNeareststops);

            // Add new nearest stops
            if (request.NearestStops != null && request.NearestStops.Count > 0)
            {
                foreach (var item in request.NearestStops)
                {
                    var nearestStop = new TblypsstoreNeareststop
                    {
                        StoreId = storeId,
                        MatchedStopId = item.MatchedStopId,
                        StopNameMm = item.StopNameMm,
                        StopNameEn = item.StopNameEn
                    };
                    _context.TblypsstoreNeareststops.Add(nearestStop);
                }
            }

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true, "Nearest bus stops assigned to YPS store successfully.");
        }

        public async Task<Result<bool>> AssignServingBusLinesAsync(string storeId, AssignServingBusLinesRequest request)
        {
            var store = await _context.Tblypsstores
                .Include(s => s.TblypsstoreServingbuslines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            // Remove existing serving bus lines
            _context.TblypsstoreServingbuslines.RemoveRange(store.TblypsstoreServingbuslines);

            // Add new serving bus lines
            if (request.BusNumbers != null && request.BusNumbers.Count > 0)
            {
                foreach (var busNumber in request.BusNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(busNumber))
                    {
                        var servingBusLine = new TblypsstoreServingbusline
                        {
                            StoreId = storeId,
                            BusNumber = busNumber.Trim()
                        };
                        _context.TblypsstoreServingbuslines.Add(servingBusLine);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true, "Serving bus lines assigned to YPS store successfully.");
        }
    }
}
