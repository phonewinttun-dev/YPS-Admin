using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblYpsStoreServingBusLine
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public string BusNumber { get; set; } = null!;

    public int? RouteId { get; set; }

    public virtual TblBusLine? Route { get; set; }

    public virtual TblYpsStore Store { get; set; } = null!;
}
