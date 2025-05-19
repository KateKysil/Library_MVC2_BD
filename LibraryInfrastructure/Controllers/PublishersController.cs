using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Model;
using LibraryInfrastructure.Services;

namespace LibraryInfrastructure.Controllers
{
    public class PublishersController : Controller
    {
        private readonly LibraryContext _context;

        private readonly PublisherDataPortServiceFactory _publisherDataPortServiceFactory;

        public PublishersController(LibraryContext context)
        {
            _context = context;
            _publisherDataPortServiceFactory = new PublisherDataPortServiceFactory(_context);
        }

        // GET: Publishers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Publishers.ToListAsync());
        }
        public async Task<IActionResult> Index_Admin()
        {
            return View(await _context.Publishers.ToListAsync());
        }
        public async Task<IActionResult> Index_User()
        {
            return View(await _context.Publishers.ToListAsync());
        }

        // GET: Publishers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Books", new { id = publisher.Id, name = publisher.PublisherName, type = "publishers" });
        }

        public async Task<IActionResult> Details_Admin(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index_Admin", "Books", new { id = publisher.Id, name = publisher.PublisherName, type = "publishers" });
        }
        public async Task<IActionResult> Details_User(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index_User", "Books", new { id = publisher.Id, name = publisher.PublisherName, type = "publishers" });
        }

        // GET: Publishers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Publishers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Country,PublisherName,Id")] Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publisher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        // GET: Publishers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }

        // POST: Publishers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Country,PublisherName,Id")] Publisher publisher)
        {
            if (id != publisher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            var importService = _publisherDataPortServiceFactory.GetImportService(fileExcel.ContentType);

            using var stream = fileExcel.OpenReadStream();

            await importService.ImportFromStreamAsync(stream, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    CancellationToken cancellationToken = default)
        {
            var exportService = _publisherDataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;


            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"categories_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }
        [HttpGet]
        public async Task<IActionResult> ExportDocx(CancellationToken cancellationToken)
        {
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var exportService = _publisherDataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream(); // don't use `using` here

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            memoryStream.Position = 0; // reset for reading

            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"publishers_{DateTime.UtcNow:yyyy-MM-dd}.docx"
            };
        }


        // GET: Publishers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher != null)
            {
                var books = _context.Books.Where(b => b.PublisherId == id).ToList();

                foreach (var book in books)
                {
                    var bookAuthors = _context.BookAuthors.Where(ba => ba.BookId == book.Id);
                    _context.BookAuthors.RemoveRange(bookAuthors);
                    var bookGenres = _context.BookGenres.Where(bg => bg.BookId == book.Id);
                    _context.BookGenres.RemoveRange(bookGenres);
                    var shelfBooks = _context.ShelfBooks.Where(sb => sb.BookId == book.Id);
                    _context.ShelfBooks.RemoveRange(shelfBooks);
                    _context.Books.Remove(book);
                }
                _context.Publishers.Remove(publisher);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool PublisherExists(long id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}