using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class History
{
    public int Id { get; set; }

    public int? IdProduct { get; set; }

    public int? Quantity { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
