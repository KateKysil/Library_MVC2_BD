using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Author : Entity
{
    [Display(Name = "Ім'я")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string FirstName { get; set; } = null!;
    [Display(Name = "Прізвище")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string LastName { get; set; } = null!;
    [Display(Name = "Країна")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Country { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}