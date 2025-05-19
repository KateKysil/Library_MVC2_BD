using ClosedXML.Excel;
using LibraryDomain.Model;
using LibraryInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace LibraryInfrastructure.Services
{
    public class PublisherExportService : IExportService<Publisher>
    {
        private readonly LibraryContext _context;

        public PublisherExportService(LibraryContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            using var workbook = new XLWorkbook();

            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .ThenInclude(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .ToListAsync(cancellationToken);

            foreach (var publisher in publishers)
            {
                var worksheet = workbook.Worksheets.Add(publisher.PublisherName);

                worksheet.Cell(1, 1).Value = "Title";
                worksheet.Cell(1, 2).Value = "ISBN";
                worksheet.Cell(1, 3).Value = "Authors";

                worksheet.Row(1).Style.Font.Bold = true;

                int row = 2;

                foreach (var book in publisher.Books)
                {
                    worksheet.Cell(row, 1).Value = book.Title;
                    worksheet.Cell(row, 2).Value = book.Isbn;

                    var authors = book.BookAuthors.Select(ba => ba.Author.FirstName + " " + ba.Author.LastName);
                    worksheet.Cell(row, 3).Value = string.Join(", ", authors);
                    row++;
                }

                worksheet.Columns().AdjustToContents();
            }

            workbook.SaveAs(stream);
            stream.Position = 0;
        }
    }
}