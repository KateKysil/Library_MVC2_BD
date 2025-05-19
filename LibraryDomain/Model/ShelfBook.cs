using System;
using System.Collections.Generic;

namespace LibraryDomain.Model;

public partial class ShelfBook : Entity
{

    public long BookId { get; set; }

    public long ShelfId { get; set; }

    public string? Comment { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Shelf Shelf { get; set; } = null!;
}