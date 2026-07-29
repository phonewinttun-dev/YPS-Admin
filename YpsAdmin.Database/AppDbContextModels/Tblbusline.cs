using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class Tblbusline
{
    public string RouteId { get; set; } = null!;

    public string BusNumber { get; set; } = null!;

    public string? OutboundTitleMm { get; set; }

    public string? OutboundTitleEn { get; set; }

    public string? ReturnTitleMm { get; set; }

    public string? ReturnTitleEn { get; set; }

    public bool? IsYpsAccepted { get; set; }

    public virtual ICollection<Tblroutestop> Tblroutestops { get; set; } = new List<Tblroutestop>();
}
