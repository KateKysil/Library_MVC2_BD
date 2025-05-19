using DocumentFormat.OpenXml.Wordprocessing;
using LibraryDomain.Model;
using LibraryInfrastructure;
using LibraryInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace LibraryInfrastructure.Services
{
    public class PublisherDocxExportService : IExportService<Publisher>
    {
        private readonly LibraryContext _context;

        public PublisherDocxExportService(LibraryContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            using var tempStream = new MemoryStream();
            var document = DocX.Create(tempStream);

            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .ThenInclude(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .ToListAsync(cancellationToken);

            foreach (var publisher in publishers)
            {
                if (publisher != publishers.First())
                {
                    document.InsertParagraph().InsertPageBreakAfterSelf();
                }
                document.InsertParagraph(publisher.PublisherName)
                        .FontSize(16)
                        .Bold()
                        .SpacingAfter(10);

                var table = document.AddTable(publisher.Books.Count + 1, 3);
                table.Design = TableDesign.ColorfulList;

                table.Rows[0].Cells[0].Paragraphs[0].Append("Title").Bold();
                table.Rows[0].Cells[1].Paragraphs[0].Append("ISBN").Bold();
                table.Rows[0].Cells[2].Paragraphs[0].Append("Authors").Bold();

                int row = 1;
                foreach (var book in publisher.Books)
                {
                    table.Rows[row].Cells[0].Paragraphs[0].Append(book.Title);
                    table.Rows[row].Cells[1].Paragraphs[0].Append(book.Isbn);
                    var authors = book.BookAuthors.Select(a => $"{a.Author.FirstName} {a.Author.LastName}");
                    table.Rows[row].Cells[2].Paragraphs[0].Append(string.Join(", ", authors));
                    row++;
                }

                document.InsertTable(table);
                document.InsertParagraph();
            }

            document.Save();

            tempStream.Position = 0;
            await tempStream.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;
        }

    }
}