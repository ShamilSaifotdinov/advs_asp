using System;
using System.Collections.Generic;

namespace advs_backend.DB;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Adv> Advs { get; } = new List<Adv>();
}
