using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class Client
{
    public int Id { get; set; }

    public int? IdClientType { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal? PersonalDiscount { get; set; }

    public virtual ClientType? IdClientTypeNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
