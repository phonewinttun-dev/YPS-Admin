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
            var query = _context.TblYpsStores.AsNoTracking().Include(s => s.Township).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchName))
            {
                string search = filter.SearchName.Trim().ToLower();
                query = query.Where(s =>
                    s.NameMm.ToLower().Contains(search) ||
                    (s.NameEn != null && s.NameEn.ToLower().Contains(search)));
            }

            if (filter.TownshipId.HasValue)
            {
                query = query.Where(s => s.TownshipId == filter.TownshipId.Value);
            }

            int totalCount = await query.CountAsync();

            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var stores = await query
                .Include(s => s.TblYpsStoreNearestStops)
                .Include(s => s.TblYpsStoreServingBusLines)
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
                TownshipId = s.TownshipId,
                TownshipNameMm = s.Township?.TownshipNameMm,
                TownshipNameEn = s.Township?.TownshipNameEn,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                NearestStops = s.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = s.TblYpsStoreServingBusLines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            }).ToList();

            var pagination = new Pagination(filter.PageNumber, filter.PageSize, totalCount);
            return PagedResult<YpsStoreDto>.Success(dtos, pagination, "YPS stores retrieved successfully.");
        }

        public async Task<Result<YpsStoreDto>> GetYpsStoreByIdAsync(int storeId)
        {
            var store = await _context.TblYpsStores
                .AsNoTracking()
                .Include(s => s.Township)
                .Include(s => s.TblYpsStoreNearestStops)
                .Include(s => s.TblYpsStoreServingBusLines)
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
                TownshipId = store.TownshipId,
                TownshipNameMm = store.Township?.TownshipNameMm,
                TownshipNameEn = store.Township?.TownshipNameEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                NearestStops = store.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = store.TblYpsStoreServingBusLines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store retrieved successfully.");
        }

        public async Task<Result<YpsStoreDto>> CreateYpsStoreAsync(CreateYpsStoreRequest request)
        {
            Point? geom = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                geom = new Point((double)request.Longitude.Value, (double)request.Latitude.Value)
                {
                    SRID = 4326
                };
            }

            var store = new TblYpsStore
            {
                NameMm = request.NameMm?.Trim() ?? string.Empty,
                NameEn = request.NameEn?.Trim(),
                Category = request.Category?.Trim(),
                TownshipId = request.TownshipId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Geom = geom
            };

            if (request.StoreId.HasValue && request.StoreId.Value > 0)
            {
                bool exists = await _context.TblYpsStores.AnyAsync(s => s.StoreId == request.StoreId.Value);
                if (exists)
                {
                    return Result<YpsStoreDto>.Failure($"A YPS store with Store ID '{request.StoreId.Value}' already exists.");
                }
                store.StoreId = request.StoreId.Value;
            }

            _context.TblYpsStores.Add(store);
            await _context.SaveChangesAsync();

            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (store.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(store.TownshipId.Value);
                townshipNameMm = township?.TownshipNameMm;
                townshipNameEn = township?.TownshipNameEn;
            }

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipId = store.TownshipId,
                TownshipNameMm = townshipNameMm,
                TownshipNameEn = townshipNameEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store created successfully.");
        }

        public async Task<Result<YpsStoreDto>> UpdateYpsStoreAsync(int storeId, UpdateYpsStoreRequest request)
        {
            var store = await _context.TblYpsStores
                .Include(s => s.Township)
                .Include(s => s.TblYpsStoreNearestStops)
                .Include(s => s.TblYpsStoreServingBusLines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<YpsStoreDto>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            store.NameMm = request.NameMm?.Trim() ?? store.NameMm;
            store.NameEn = request.NameEn?.Trim();
            store.Category = request.Category?.Trim();
            store.TownshipId = request.TownshipId;
            store.Latitude = request.Latitude;
            store.Longitude = request.Longitude;

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

            string? townshipNameMm = null;
            string? townshipNameEn = null;
            if (store.TownshipId.HasValue)
            {
                var township = await _context.TblTownships.FindAsync(store.TownshipId.Value);
                townshipNameMm = township?.TownshipNameMm;
                townshipNameEn = township?.TownshipNameEn;
            }

            var dto = new YpsStoreDto
            {
                StoreId = store.StoreId,
                NameMm = store.NameMm,
                NameEn = store.NameEn,
                Category = store.Category,
                TownshipId = store.TownshipId,
                TownshipNameMm = townshipNameMm,
                TownshipNameEn = townshipNameEn,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                NearestStops = store.TblYpsStoreNearestStops.Select(ns => new NearestStopDto
                {
                    Id = ns.Id,
                    StopNameMm = ns.StopNameMm,
                    StopNameEn = ns.StopNameEn,
                    MatchedStopId = ns.MatchedStopId
                }).ToList(),
                ServingBusLines = store.TblYpsStoreServingBusLines
                    .Select(sl => sl.BusNumber)
                    .ToList()
            };

            return Result<YpsStoreDto>.Success(dto, "YPS store updated successfully.");
        }

        public async Task<Result<bool>> DeleteYpsStoreAsync(int storeId)
        {
            var store = await _context.TblYpsStores.FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            _context.TblYpsStores.Remove(store);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true, "YPS store deleted successfully.");
        }

        public async Task<Result<bool>> AssignNearestStopsAsync(int storeId, AssignNearestStopsRequest request)
        {
            var store = await _context.TblYpsStores
                .Include(s => s.TblYpsStoreNearestStops)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            _context.TblYpsStoreNearestStops.RemoveRange(store.TblYpsStoreNearestStops);

            if (request.NearestStops != null && request.NearestStops.Count > 0)
            {
                foreach (var item in request.NearestStops)
                {
                    var nearestStop = new TblYpsStoreNearestStop
                    {
                        StoreId = storeId,
                        MatchedStopId = item.MatchedStopId,
                        StopNameMm = item.StopNameMm,
                        StopNameEn = item.StopNameEn
                    };
                    _context.TblYpsStoreNearestStops.Add(nearestStop);
                }
            }

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true, "Nearest bus stops assigned to YPS store successfully.");
        }

        public async Task<Result<bool>> AssignServingBusLinesAsync(int storeId, AssignServingBusLinesRequest request)
        {
            var store = await _context.TblYpsStores
                .Include(s => s.TblYpsStoreServingBusLines)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);

            if (store == null)
            {
                return Result<bool>.Failure($"YPS store with Store ID '{storeId}' was not found.");
            }

            _context.TblYpsStoreServingBusLines.RemoveRange(store.TblYpsStoreServingBusLines);

            if (request.BusNumbers != null && request.BusNumbers.Count > 0)
            {
                foreach (var busNumber in request.BusNumbers)
                {
                    // Find RouteId if bus number matches a bus line
                    var busLine = await _context.TblBusLines.FirstOrDefaultAsync(b => b.BusNumber == busNumber);

                    var servingBusLine = new TblYpsStoreServingBusLine
                    {
                        StoreId = storeId,
                        BusNumber = busNumber,
                        RouteId = busLine?.RouteId
                    };
                    _context.TblYpsStoreServingBusLines.Add(servingBusLine);
                }
            }

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true, "Serving bus lines assigned to YPS store successfully.");
        }
    }
}
