using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryDomain.Model;
using LibraryInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace LibraryInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private record CountByAuthorBooksItem(string Author, int Count);
        private readonly LibraryContext libraryContext;

        public ChartController(LibraryContext libraryContext)
        {
            this.libraryContext = libraryContext;
        }
        [HttpGet("countByAuthor")]
        public async Task<JsonResult> GetCountByAuthorAsync(CancellationToken cancellationToken)
        {
            var responseItems = await libraryContext.Books
                .SelectMany(book => book.BookAuthors)
                .GroupBy(bookAuthor => new
                {
                    bookAuthor.Author.FirstName,
                    bookAuthor.Author.LastName
                })
                .Select(group => new CountByAuthorBooksItem(
                    $"{group.Key.LastName} {group.Key.FirstName}",
                    group.Count()
                ))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }

        [HttpGet("countByPublisher")]
        public async Task<JsonResult> GetCountByPublisherAsync(CancellationToken cancellationToken)
        {
            var responseItems = await libraryContext.Books
                .GroupBy(book => book.Publisher.PublisherName)
                .Select(group => new
                {
                    Publisher = group.Key,
                    Count = group.Count()
                })
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }
    }


}