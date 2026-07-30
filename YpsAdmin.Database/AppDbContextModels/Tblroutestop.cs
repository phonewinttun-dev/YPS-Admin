using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblRouteStop
{
    public int Id { get; set; }

    public int RouteId { get; set; }

    public int? StopId { get; set; }

    public string Direction { get; set; } = null!;

    public int StopOrder { get; set; }

    public string? StopType { get; set; }

    public virtual TblBusLine Route { get; set; } = null!;

    public virtual TblBusStop? Stop { get; set; }
}
