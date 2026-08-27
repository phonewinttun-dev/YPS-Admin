using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblNearestBusStop
{
    public long Id { get; set; }

    public int StoreId { get; set; }

    public long BusStopId { get; set; }

    public double? DistanceKm { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual TblStore Store { get; set; } = null!;

    public virtual TblBusStop BusStop { get; set; } = null!;
}
