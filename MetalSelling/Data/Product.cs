using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class Product
{
    public int Id { get; set; }

    public int? IdMetalType { get; set; }

    public string? Title { get; set; }

    public int? Amount { get; set; }

    public decimal? PricePerPiece { get; set; }

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual MetalType? IdMetalTypeNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
