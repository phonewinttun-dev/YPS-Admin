using System;
using System.Collections.Generic;

namespace YpsAdmin.Database.AppDbContextModels;

public partial class TblypsstoreServingbusline
{
    public int Id { get; set; }

    public string StoreId { get; set; } = null!;

    public string BusNumber { get; set; } = null!;

    public virtual Tblypsstore Store { get; set; } = null!;
}
