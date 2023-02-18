using System;
using System.Collections.Generic;

namespace advs_backend.DB;

public partial class Adv
{
    public int AdvId { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public string? Discription { get; set; }

    public decimal? Price { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
