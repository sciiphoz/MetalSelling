using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class Order
{
    public int Id { get; set; }

    public int? IdProduct { get; set; }

    public int? IdClient { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Client? IdClientNavigation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
