using System;
using YpsAdmin.Shared;

namespace YpsAdmin.Domain.DTOs.BusStop
{
    public class BusStopDto
    {
        public long Id { get; set; }
        public string StopName { get; set; } = null!;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
        public string? RegionName { get; set; }
        public bool DeleteFlag { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly UpdatedAt { get; set; }
    }

    public class CreateBusStopRequest
    {
        public string StopName { get; set; } = null!;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
    }

    public class UpdateBusStopRequest
    {
        public string StopName { get; set; } = null!;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
        public bool? DeleteFlag { get; set; }
    }

    public class BusStopGetRequest : PaginationRequest
    {
        public int? RegionId { get; set; }
    }

    public class BusStopSearchRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
        public int? RegionId { get; set; }
    }
}
