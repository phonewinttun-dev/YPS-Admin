using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblRegion
{
    public int Id { get; set; }

    public string RegionName { get; set; } = null!;

    public bool? DeleteFlag { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual ICollection<TblBusStop> TblBusStops { get; set; } = new List<TblBusStop>();

    public virtual ICollection<TblStore> TblStores { get; set; } = new List<TblStore>();
}
