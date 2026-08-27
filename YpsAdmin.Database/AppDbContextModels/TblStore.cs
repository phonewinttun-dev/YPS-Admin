using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblStore
{
    public int Id { get; set; }

    public string? EngName { get; set; }

    public string? MmName { get; set; }

    public string? Category { get; set; }

    public double Lat { get; set; }

    public double Lon { get; set; }

    public int? RegionId { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public virtual TblRegion? Region { get; set; }

    public virtual ICollection<TblNearestBusStop> TblNearestBusStops { get; set; } = new List<TblNearestBusStop>();
}
