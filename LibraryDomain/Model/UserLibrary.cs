using System;
using System.Collections.Generic;

namespace LibraryDomain.Model;

public partial class UserLibrary : Entity
{
    public string UserId { get; set; }

    public long BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}