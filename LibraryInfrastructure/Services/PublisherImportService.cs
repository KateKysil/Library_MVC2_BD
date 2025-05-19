using ClosedXML.Excel;
using LibraryDomain.Model;
using LibraryInfrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Services
{
    public class PublisherImportService : IImportService<Publisher>
    {
        private readonly LibraryContext _context;

        public PublisherImportService(LibraryContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    var publisherName = worksheet.Name;

                    var publisher = await _context.Publishers
                        .FirstOrDefaultAsync(p => p.PublisherName == publisherName, cancellationToken);

                    if (publisher == null)
                    {
                        publisher = new Publisher
                        {
                            PublisherName = publisherName,
                            Country = ""
                        };
                        _context.Publishers.Add(publisher);
                    }

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        try
                        {
                            await AddBookAsync(row, cancellationToken, publisher);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Skipped row due to duplicate or null ISBN: {ex.Message}");
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task AddBookAsync(IXLRow row, CancellationToken cancellationToken, Publisher publisher)
        {
            bool isbnExists = await _context.Books.AnyAsync(b => b.Isbn == GetBookIsbn(row), cancellationToken);
            if (isbnExists)
            {
                throw new InvalidOperationException($"A book with ISBN '{GetBookIsbn(row)}' already exists.");
            }
            if (GetBookIsbn(row) == "")
            {
                throw new InvalidOperationException($"ISBN for {GetBookName(row)} is null");
            }
            var book = new Book
            {
                Title = GetBookName(row),
                Isbn = GetBookIsbn(row),
                Publisher = publisher
            };
            _context.Books.Add(book);
            await GetAuthorsAsync(row, book, cancellationToken);
        }

        private static string GetBookName(IXLRow row) => row.Cell(1).GetString();
        private static string GetBookIsbn(IXLRow row) => row.Cell(2).GetString();

        private async Task GetAuthorsAsync(IXLRow row, Book book, CancellationToken cancellationToken)
        {
            for (int i = 3; i <= 5; i++)
            {
                var authorName = row.Cell(i).GetString().Trim();
                if (!string.IsNullOrEmpty(authorName))
                {
                    var nameParts = authorName.Split(' ', 2);
                    var firstName = nameParts.Length > 0 ? nameParts[0] : "Unknown";
                    var lastName = nameParts.Length > 1 ? nameParts[1] : "Unknown";

                    var author = await _context.Authors
                        .FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName, cancellationToken);

                    if (author == null)
                    {
                        author = new Author
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Country = "from EXCEL"
                        };
                        _context.Authors.Add(author);
                    }

                    _context.BookAuthors.Add(new BookAuthor { Book = book, Author = author });
                }
            }
        }
    }
}