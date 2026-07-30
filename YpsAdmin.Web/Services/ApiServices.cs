using System.Net.Http.Json;
using YpsAdmin.Shared;

namespace YpsAdmin.Web.Services;

// DTOs matching API specs
public record BusLineDto(int BusLineId, string RouteId, string BusNumber, string OutboundTitles, string ReturnTitles, bool YpsAccepted);
public record CreateBusLineRequest(string RouteId, string BusNumber, string OutboundTitles, string ReturnTitles, bool YpsAccepted);
public record UpdateBusLineRequest(string RouteId, string BusNumber, string OutboundTitles, string ReturnTitles, bool YpsAccepted);

public record BusStopDto(int BusStopId, string StopId, string NameMm, string NameEn, string TownshipMm, string TownshipEn, string Road);
public record CreateBusStopRequest(string StopId, string NameMm, string NameEn, string TownshipMm, string TownshipEn, string Road);
public record UpdateBusStopRequest(string StopId, string NameMm, string NameEn, string TownshipMm, string TownshipEn, string Road);

public record RouteStopDto(int RouteStopId, int BusLineId, int BusStopId, string Direction, int StopOrder, BusStopDto? BusStop);
public record FullRouteResponseDto(int BusLineId, string BusNumber, List<RouteStopDto> OutboundStops, List<RouteStopDto> ReturnStops);
public record AssignRouteStopsRequest(int BusLineId, List<int> BusStopIds, string Direction);
public record ReorderRouteStopsRequest(int BusLineId, string Direction, List<int> RouteStopIds);

public record YpsStoreDto(int YpsStoreId, string StoreName, string Category, string Township, double Latitude, double Longitude, List<string> NearestStops, List<string> ServingBusLines);
public record CreateYpsStoreRequest(string StoreName, string Category, string Township, double Latitude, double Longitude);
public record UpdateYpsStoreRequest(string StoreName, string Category, string Township, double Latitude, double Longitude);
public record AssignNearestStopsRequest(List<int> BusStopIds);
public record AssignServingBusLinesRequest(List<int> BusLineIds);

// Services Interfaces
public interface IBusLineService
{
    Task<PagedResult<BusLineDto>?> GetBusLinesAsync(int pageNumber, int pageSize, string? searchBusNumber);
    Task<Result<BusLineDto>?> GetByIdAsync(int id);
    Task<Result<BusLineDto>?> CreateAsync(CreateBusLineRequest request);
    Task<Result<BusLineDto>?> UpdateAsync(int id, UpdateBusLineRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
}

public interface IBusStopService
{
    Task<PagedResult<BusStopDto>?> GetBusStopsAsync(int pageNumber, int pageSize, string? searchStopName);
    Task<Result<BusStopDto>?> GetByIdAsync(int id);
    Task<Result<BusStopDto>?> CreateAsync(CreateBusStopRequest request);
    Task<Result<BusStopDto>?> UpdateAsync(int id, UpdateBusStopRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
}

public interface IRouteStopService
{
    Task<Result<FullRouteResponseDto>> GetFullRouteAsync(int busLineId);
    Task<Result<bool>> AssignRouteStopsAsync(AssignRouteStopsRequest request);
    Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request);
    Task<Result<bool>> DeleteRouteStopAsync(int routeStopId);
}

public interface IYpsStoreService
{
    Task<PagedResult<YpsStoreDto>?> GetStoresAsync(int pageNumber, int pageSize, string? searchName);
    Task<Result<YpsStoreDto>?> GetByIdAsync(int id);
    Task<Result<YpsStoreDto>?> CreateAsync(CreateYpsStoreRequest request);
    Task<Result<YpsStoreDto>?> UpdateAsync(int id, UpdateYpsStoreRequest request);
    Task<Result<bool>?> DeleteAsync(int id);
    Task<Result<bool>?> AssignNearestStopsAsync(int id, AssignNearestStopsRequest request);
    Task<Result<bool>?> AssignServingBusLinesAsync(int id, AssignServingBusLinesRequest request);
}

// Service Implementations
public class BusLineService : IBusLineService
{
    private readonly HttpClient _http;
    public BusLineService(HttpClient http) => _http = http;

    public Task<PagedResult<BusLineDto>?> GetBusLinesAsync(int pageNumber, int pageSize, string? searchBusNumber)
    {
        var url = $"/api/bus-lines?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchBusNumber)) url += $"&searchBusNumber={Uri.EscapeDataString(searchBusNumber)}";
        return _http.GetFromJsonAsync<PagedResult<BusLineDto>>(url);
    }

    public Task<Result<BusLineDto>?> GetByIdAsync(int id) => _http.GetFromJsonAsync<Result<BusLineDto>>($"/api/bus-lines/{id}");

    public async Task<Result<BusLineDto>?> CreateAsync(CreateBusLineRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/api/bus-lines", request);
        return await resp.Content.ReadFromJsonAsync<Result<BusLineDto>>();
    }

    public async Task<Result<BusLineDto>?> UpdateAsync(int id, UpdateBusLineRequest request)
    {
        var resp = await _http.PutAsJsonAsync($"/api/bus-lines/{id}", request);
        return await resp.Content.ReadFromJsonAsync<Result<BusLineDto>>();
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/bus-lines/{id}");
        return await resp.Content.ReadFromJsonAsync<Result<bool>>();
    }
}

public class BusStopService : IBusStopService
{
    private readonly HttpClient _http;
    public BusStopService(HttpClient http) => _http = http;

    public Task<PagedResult<BusStopDto>?> GetBusStopsAsync(int pageNumber, int pageSize, string? searchStopName)
    {
        var url = $"/api/bus-stops?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchStopName)) url += $"&searchStopName={Uri.EscapeDataString(searchStopName)}";
        return _http.GetFromJsonAsync<PagedResult<BusStopDto>>(url);
    }

    public Task<Result<BusStopDto>?> GetByIdAsync(int id) => _http.GetFromJsonAsync<Result<BusStopDto>>($"/api/bus-stops/{id}");

    public async Task<Result<BusStopDto>?> CreateAsync(CreateBusStopRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/api/bus-stops", request);
        return await resp.Content.ReadFromJsonAsync<Result<BusStopDto>>();
    }

    public async Task<Result<BusStopDto>?> UpdateAsync(int id, UpdateBusStopRequest request)
    {
        var resp = await _http.PutAsJsonAsync($"/api/bus-stops/{id}", request);
        return await resp.Content.ReadFromJsonAsync<Result<BusStopDto>>();
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/bus-stops/{id}");
        return await resp.Content.ReadFromJsonAsync<Result<bool>>();
    }
}

public class RouteStopService : IRouteStopService
{
    private readonly HttpClient _http;
    public RouteStopService(HttpClient http) => _http = http;

    public async Task<Result<FullRouteResponseDto>> GetFullRouteAsync(int busLineId)
    {
        return (await _http.GetFromJsonAsync<Result<FullRouteResponseDto>>($"/api/route-stops/bus-line/{busLineId}"))!;
    }

    public async Task<Result<bool>> AssignRouteStopsAsync(AssignRouteStopsRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/api/route-stops/assign", request);
        return (await resp.Content.ReadFromJsonAsync<Result<bool>>())!;
    }

    public async Task<Result<bool>> ReorderRouteStopsAsync(ReorderRouteStopsRequest request)
    {
        var resp = await _http.PutAsJsonAsync("/api/route-stops/reorder", request);
        return (await resp.Content.ReadFromJsonAsync<Result<bool>>())!;
    }

    public async Task<Result<bool>> DeleteRouteStopAsync(int routeStopId)
    {
        var resp = await _http.DeleteAsync($"/api/route-stops/{routeStopId}");
        return (await resp.Content.ReadFromJsonAsync<Result<bool>>())!;
    }
}

public class YpsStoreService : IYpsStoreService
{
    private readonly HttpClient _http;
    public YpsStoreService(HttpClient http) => _http = http;

    public Task<PagedResult<YpsStoreDto>?> GetStoresAsync(int pageNumber, int pageSize, string? searchName)
    {
        var url = $"/api/yps-stores?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchName)) url += $"&searchName={Uri.EscapeDataString(searchName)}";
        return _http.GetFromJsonAsync<PagedResult<YpsStoreDto>>(url);
    }

    public Task<Result<YpsStoreDto>?> GetByIdAsync(int id) => _http.GetFromJsonAsync<Result<YpsStoreDto>>($"/api/yps-stores/{id}");

    public async Task<Result<YpsStoreDto>?> CreateAsync(CreateYpsStoreRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/api/yps-stores", request);
        return await resp.Content.ReadFromJsonAsync<Result<YpsStoreDto>>();
    }

    public async Task<Result<YpsStoreDto>?> UpdateAsync(int id, UpdateYpsStoreRequest request)
    {
        var resp = await _http.PutAsJsonAsync($"/api/yps-stores/{id}", request);
        return await resp.Content.ReadFromJsonAsync<Result<YpsStoreDto>>();
    }

    public async Task<Result<bool>?> DeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/yps-stores/{id}");
        return await resp.Content.ReadFromJsonAsync<Result<bool>>();
    }

    public async Task<Result<bool>?> AssignNearestStopsAsync(int id, AssignNearestStopsRequest request)
    {
        var resp = await _http.PostAsJsonAsync($"/api/yps-stores/{id}/nearest-stops", request);
        return await resp.Content.ReadFromJsonAsync<Result<bool>>();
    }

    public async Task<Result<bool>?> AssignServingBusLinesAsync(int id, AssignServingBusLinesRequest request)
    {
        var resp = await _http.PostAsJsonAsync($"/api/yps-stores/{id}/serving-bus-lines", request);
        return await resp.Content.ReadFromJsonAsync<Result<bool>>();
    }
}
