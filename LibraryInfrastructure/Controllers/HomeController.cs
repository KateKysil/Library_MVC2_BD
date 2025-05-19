using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryDomain.Model;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Net;

namespace LibraryInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly LibraryContext _context;
        public HomeController(LibraryContext context,UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return RedirectToAction("Home", "Home", new { id = currentUser.Id });
            }
            else
            {
                return RedirectToAction("Details_User", "Users", new { id = currentUser.Id });
            }
        }
        public async Task<IActionResult> Home()
        {
            return View();
        }
        public async Task<IActionResult> Tables()
        {
            return View();
        }

        public IActionResult Requests(int? id, string? publisherName = null, string? genreName = null, string? authorName=null, int? authorId=null, int? bookId=null)
        {
            var model = new RequestViewModel();

            switch (id)
            {
                case 1:
                    ViewData["PublisherName"] = publisherName;
                    if (string.IsNullOrWhiteSpace(publisherName))
                        publisherName = "Темпора";
                    model.RequestTitle = "Книги видавництва " + publisherName;
                    var result1 = _context.Books
                    .Where(b => b.Publisher.PublisherName == publisherName)
                    .Select(b => new { b.Title, Publisher = b.Publisher.PublisherName })
                    .ToList();

                    model.Headers = new List<string> { "Назва книги", "Видавництво" };
                    model.Results = result1.Select(r => new List<string> { r.Title, r.Publisher }).ToList();
                    break;

                case 2:
                    ViewData["GenreName"] = genreName;
                    if (string.IsNullOrWhiteSpace(genreName))
                        genreName = "Romance";
                    model.RequestTitle = "Автори жанру " + genreName;
                    var result2 = _context.Authors
                        .Where(a => a.BookAuthors
                            .Any(ba => ba.Book.BookGenres
                                .Any(bg => bg.Genre.GenreName == genreName)))
                        .Select(a => new { FullName= a.LastName+" "+ a.FirstName})
                        .Distinct()
                        .ToList();

                    model.Headers = new List<string> { "Автор" };
                    model.Results = result2.Select(r => new List<string> { r.FullName }).ToList();
                    break;

                case 3:
                    model.RequestTitle = "Книги авторів, які написали більше однієї книги";

                    // Get author IDs who have written more than one book
                    var authorsWithMultipleBooks = _context.BookAuthors
                        .GroupBy(ba => ba.AuthorId)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToHashSet();

                    // Get books that have at least one such author
                    var result = _context.Books
                        .Where(b => b.BookAuthors.Any(ba => authorsWithMultipleBooks.Contains(ba.AuthorId)))
                        .Select(b => new
                        {
                            b.Title,
                            b.Isbn,
                            Authors = b.BookAuthors
                                .Where(ba => authorsWithMultipleBooks.Contains(ba.AuthorId))
                                .Select(ba => ba.Author.LastName + " " + ba.Author.FirstName)
                        })
                        .AsEnumerable() 
                        .Select(b => new
                        {
                            b.Title,
                            b.Isbn,
                            Authors = b.Authors.Distinct().ToList()
                        })
                        .ToList();

                    model.Headers = new List<string> { "Назва книги", "ISBN", "Автор(и)" };
                    model.Results = result.Select(r => new List<string>
                    {
                        r.Title,
                        r.Isbn,
                        string.Join(", ", r.Authors)
                    }).ToList();

                    break;

                case 4:
                    ViewData["GenreName"] = genreName;
                    if (string.IsNullOrWhiteSpace(genreName))
                        genreName = "Romance";
                    ViewData["PublisherName"] = publisherName;
                    if (string.IsNullOrWhiteSpace(publisherName))
                        publisherName = "Темпора";
                    model.RequestTitle = "Книги видавництва " + publisherName + " і жанру " + genreName;
                    var result4 = _context.Books
                    .Where(b => b.Publisher.PublisherName == publisherName &&
                                b.BookGenres.Any(bg => bg.Genre.GenreName == genreName))
                    .Select(b => new {
                        b.Title,
                        Publisher = b.Publisher.PublisherName
                    })
                    .ToList();
                    model.Headers = new List<string> { "Назва книги", "Видавництво" };
                    model.Results = result4.Select(r => new List<string> { r.Title, r.Publisher }).ToList();
                    break;
                case 5:
                    ViewData["AuthorName"] = authorName;
                    if (string.IsNullOrWhiteSpace(authorName))
                        authorName = "Don";
                    model.RequestTitle = "Книги автора з ім'ям або прізвищем " +authorName;
                    var result5 = _context.Authors
                    .Where(a => a.FirstName == authorName || a.LastName == authorName)
                    .Select(a => new {
                        a.FirstName,
                        a.LastName,
                        a.Country
                    })
                    .ToList();
                    model.Headers = new List<string> { "Ім'я", "Прізвище", "Країна" };
                    model.Results = result5.Select(r => new List<string> { r.FirstName, r.LastName, r.Country }).ToList();
                    break;
                case 6:
                    ViewData["AuthorId"] = authorId;
                    if (authorId == null)
                        authorId = 1;

                    model.RequestTitle = "Автори з таким самим набором книг, як у автора з Id " + authorId;

                    var baseBooks = _context.BookAuthors
                        .Where(ba => ba.AuthorId == authorId)
                        .Select(ba => ba.BookId)
                        .ToHashSet();

                    var authors1 = _context.Authors
                        .Where(a => a.Id != authorId)
                        .Include(a => a.BookAuthors)
                        .ToList();

                    var result6 = authors1
                        .Where(a => a.BookAuthors.Select(ba => ba.BookId).ToHashSet().SetEquals(baseBooks))
                        .Select(a => new {
                            a.FirstName,
                            a.LastName
                        })
                        .ToList();

                    model.Headers = new List<string> { "Ім'я", "Прізвище" };
                    model.Results = result6.Select(r => new List<string> { r.FirstName, r.LastName }).ToList();
                    break;



                case 7:
                    ViewData["bookId"] = bookId;
                    if (bookId == null)
                        bookId = 1;
                    model.RequestTitle = "Книги з такими ж жанрами, як і книга з Id " + bookId;
                    var baseGenres = _context.BookGenres
                    .Where(bg => bg.BookId == bookId)
                    .Select(bg => bg.GenreId)
                    .ToHashSet();

                    var books = _context.Books
                        .Where(b => b.Id != bookId)
                        .ToList(); 

                    var result7 = books
                        .Where(b =>
                        {
                            var genres = _context.BookGenres
                                .Where(bg => bg.BookId == b.Id)
                                .Select(bg => bg.GenreId)
                                .ToHashSet();

                            return genres.SetEquals(baseGenres);
                        })
                        .ToList();

                    model.Headers = new List<string> { "Назва книги" };
                    model.Results = result7.Select(r => new List<string> { r.Title }).ToList();
                    break;
                case 8:
                    var authors = _context.Authors.ToList();

                    var result8 = new List<(string Author1, string Author2)>();

                    for (int i = 0; i < authors.Count; i++)
                    {
                        var booksA = _context.BookAuthors
                            .Where(ba => ba.AuthorId == authors[i].Id)
                            .Select(ba => ba.BookId)
                            .ToHashSet();

                        for (int j = i + 1; j < authors.Count; j++)
                        {
                            var booksB = _context.BookAuthors
                                .Where(ba => ba.AuthorId == authors[j].Id)
                                .Select(ba => ba.BookId)
                                .ToHashSet();

                            if (booksA.SetEquals(booksB))
                            {
                                result8.Add((
                                    authors[i].FirstName + " " + authors[i].LastName,
                                    authors[j].FirstName + " " + authors[j].LastName));
                            }
                        }
                    }
                    model.Headers = new List<string> { "Назва книги" };
                    model.Results = result8.Select(r => new List<string> { r.Author1, r.Author2 }).ToList();
                    break;
                default:
                    model.RequestTitle = "Виберіть запит";
                    break;
            }

            return View(model);
        }

    }
}



//отже, я хочу, щоб коли застосунок запускається, користувач бачив великий напис "Привіт! Це програма для роботи з базою данних."
//зверху будуть кнопки "Запити" і "Таблиці"