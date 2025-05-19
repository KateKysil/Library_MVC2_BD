using System;
using System.Collections.Generic;

namespace LibraryDomain.Model;

public partial class Shelf : Entity
{

    public string Name { get; set; } = null!;

    public string UserId { get; set; }

    public bool IsPrivate { get; set; }

    public virtual ICollection<ShelfBook> ShelfBooks { get; set; } = new List<ShelfBook>();

    public virtual User User { get; set; } = null!;
}