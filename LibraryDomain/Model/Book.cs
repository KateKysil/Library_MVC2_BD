using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Book : Entity
{

    public string Title { get; set; } = null!;

    public string Isbn { get; set; } = null!;
    [Display(Name = "Назва видавництва")]
    public long PublisherId { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShelfBook> ShelfBooks { get; set; } = new List<ShelfBook>();

    public virtual ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();
}