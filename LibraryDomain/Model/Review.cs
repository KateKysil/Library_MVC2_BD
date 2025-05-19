using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Review : Entity
{
    [Display(Name = "Книга")]
    public long BookId { get; set; }
    [Display(Name = "Користувач")]
    public string UserId { get; set; }

    public string Text { get; set; } = null!;

    public double? Rate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}