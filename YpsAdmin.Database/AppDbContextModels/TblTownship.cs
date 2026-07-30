using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblTownship
{
    public int TownshipId { get; set; }

    public string TownshipNameMm { get; set; } = null!;

    public string? TownshipNameEn { get; set; }

    public bool? DeleteFlag { get; set; }

    public virtual ICollection<TblBusStop> TblBusStops { get; set; } = new List<TblBusStop>();

    public virtual ICollection<TblYpsStore> TblYpsStores { get; set; } = new List<TblYpsStore>();
}
