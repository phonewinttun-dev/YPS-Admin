using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class Tblroutestop
{
    public int Id { get; set; }

    public string RouteId { get; set; } = null!;

    public string? StopId { get; set; }

    public string Direction { get; set; } = null!;

    public int StopOrder { get; set; }

    public string? StopType { get; set; }

    public virtual Tblbusline Route { get; set; } = null!;

    public virtual Tblbusstop? Stop { get; set; }
}
