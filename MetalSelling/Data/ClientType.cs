using System;
using System.Collections.Generic;

namespace MetalSelling.Data;

public partial class ClientType
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
