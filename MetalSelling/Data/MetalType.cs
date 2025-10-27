using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class MetalType
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
