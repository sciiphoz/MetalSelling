using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class Supply
{
    public int Id { get; set; }

    public int? IdSupplier { get; set; }

    public int? IdProduct { get; set; }

    public int? Amount { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Product? IdProductNavigation { get; set; }

    public virtual Supplier? IdSupplierNavigation { get; set; }
}
