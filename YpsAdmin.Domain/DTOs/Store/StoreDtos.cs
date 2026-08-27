using System;
using System.Collections.Generic;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.Store
{
    public class NearestBusStopDto
    {
        public long Id { get; set; }
        public long BusStopId { get; set; }
        public string? StopName { get; set; }
        public double? DistanceKm { get; set; }
    }

    public class StoreDto
    {
        public int Id { get; set; }
        public string? EngName { get; set; }
        public string? MmName { get; set; }
        public string? Category { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
        public string? RegionName { get; set; }
        public bool DeleteFlag { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public List<NearestBusStopDto> NearestStops { get; set; } = new();
        public List<long> ServingBusNumbers { get; set; } = new();
    }

    public class CreateStoreRequest
    {
        public string? EngName { get; set; }
        public string? MmName { get; set; }
        public string? Category { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
    }

    public class UpdateStoreRequest
    {
        public string? EngName { get; set; }
        public string? MmName { get; set; }
        public string? Category { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
        public bool? DeleteFlag { get; set; }
    }

    public class AssignNearestBusStopItem
    {
        public long BusStopId { get; set; }
        public double? DistanceKm { get; set; }
    }

    public class AssignNearestStopsRequest
    {
        public List<AssignNearestBusStopItem> NearestStops { get; set; } = new();
    }

    public class StoreGetRequest : PaginationRequest
    {
        public int? RegionId { get; set; }
    }

    public class StoreSearchRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
        public int? RegionId { get; set; }
    }
}
