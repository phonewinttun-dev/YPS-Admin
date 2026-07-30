using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblBusStop
{
    public int StopId { get; set; }

    public string NameMm { get; set; } = null!;

    public string? NameEn { get; set; }

    public int? TownshipId { get; set; }

    public string? RoadMm { get; set; }

    public string? RoadEn { get; set; }

    public int? TotalServingBusLines { get; set; }

    public virtual ICollection<TblRouteStop> TblRouteStops { get; set; } = new List<TblRouteStop>();

    public virtual ICollection<TblYpsStoreNearestStop> TblYpsStoreNearestStops { get; set; } = new List<TblYpsStoreNearestStop>();

    public virtual TblTownship? Township { get; set; }
}
