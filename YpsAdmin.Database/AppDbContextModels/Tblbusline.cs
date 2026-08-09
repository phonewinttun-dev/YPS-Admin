using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblBusLine
{
    public int RouteId { get; set; }

    public string BusNumber { get; set; } = null!;

    public string? OutboundTitleMm { get; set; }

    public string? OutboundTitleEn { get; set; }

    public string? ReturnTitleMm { get; set; }

    public string? ReturnTitleEn { get; set; }

    public bool? IsYpsAccepted { get; set; }

    public virtual ICollection<TblRouteStop> TblRouteStops { get; set; } = new List<TblRouteStop>();

    public virtual ICollection<TblYpsStoreServingBusLine> TblYpsStoreServingBusLines { get; set; } = new List<TblYpsStoreServingBusLine>();
}
