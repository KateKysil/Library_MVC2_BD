using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Genre : Entity
{

    [Display(Name = "Назва жанру")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string GenreName { get; set; } = null!;

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}