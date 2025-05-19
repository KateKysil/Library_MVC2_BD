using System;
using System.Collections.Generic;

namespace LibraryDomain.Model;

public partial class BookAuthor : Entity
{

    public long BookId { get; set; }

    public long AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}