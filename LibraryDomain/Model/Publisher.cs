using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Publisher : Entity
{
    [Display(Name = "Країна")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Country { get; set; }
    [Display(Name = "Назва видавництва")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]

    public string PublisherName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}