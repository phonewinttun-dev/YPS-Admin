using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblBusRoute
{
    public long BusId { get; set; }

    public long BusStopId { get; set; }

    public int StopOrder { get; set; }

    public virtual TblBus Bus { get; set; } = null!;

    public virtual TblBusStop BusStop { get; set; } = null!;
}
