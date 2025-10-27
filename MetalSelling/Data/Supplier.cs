using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class Supplier
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public decimal? PricePerPiece { get; set; }

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
