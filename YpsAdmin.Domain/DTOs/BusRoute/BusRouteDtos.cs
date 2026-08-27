using System.Collections.Generic;

namespace YpsAdmin.Domain.DTOs.BusRoute
{
    public class BusRouteStopItemDto
    {
        public long BusId { get; set; }
        public long BusStopId { get; set; }
        public int StopOrder { get; set; }
        public string StopName { get; set; } = null!;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int? RegionId { get; set; }
        public string? RegionName { get; set; }
    }

    public class AssignBusRouteStopItem
    {
        public long BusStopId { get; set; }
        public int StopOrder { get; set; }
    }

    public class AssignBusRoutesRequest
    {
        public long BusId { get; set; }
        public List<AssignBusRouteStopItem> Stops { get; set; } = new();
    }

    public class ReorderBusRouteItem
    {
        public long BusStopId { get; set; }
        public int OldStopOrder { get; set; }
        public int NewStopOrder { get; set; }
    }

    public class ReorderBusRoutesRequest
    {
        public long BusId { get; set; }
        public List<ReorderBusRouteItem> Items { get; set; } = new();
    }

    public class FullRouteResponseDto
    {
        public long BusId { get; set; }
        public long BusNumber { get; set; }
        public string? VariantId { get; set; }
        public bool IsCardAccepted { get; set; }
        public bool IsReversed { get; set; }
        public List<BusRouteStopItemDto> Stops { get; set; } = new();
    }
}
