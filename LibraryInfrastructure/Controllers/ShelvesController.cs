using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Model;
using LibraryInfrastructure;
using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace LibraryInfrastructure.Controllers
{
    public class ShelvesController : Controller
    {
        private readonly LibraryContext _context;

        public ShelvesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Shelves
        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                return RedirectToAction("Index_Admin");
            }
            else
            {
                return RedirectToAction("Index_User");
            }
            var libraryContext = _context.Shelves.Include(s => s.User);
            return View(await libraryContext.ToListAsync());
        }
        public async Task<IActionResult> Index_Admin()
        {
            var libraryContext = _context.Shelves.Include(s => s.User);
            return View(await libraryContext.ToListAsync());
        }
        public async Task<IActionResult> Index_User()
        {
            var libraryContext = _context.Shelves.Include(s => s.User);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Shelves/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var isAdmin = User.IsInRole("Admin");

            // Redirect accordingly
            if (isAdmin)
            {
                return RedirectToAction("Details_Admin", new { id = id });
            }
            else
            {
                return RedirectToAction("Details_User", new { id = id });
            }
            var shelf = _context.Shelves
           .Include(s => s.User)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book)
           .ThenInclude(b => b.Publisher)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book.BookAuthors)
           .ThenInclude(ba => ba.Author)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book.BookGenres)
           .ThenInclude(bg => bg.Genre)
           .FirstOrDefault(m => m.Id == id);

            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        public async Task<IActionResult> Details_User(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = _context.Shelves
           .Include(s => s.User)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book)
           .ThenInclude(b => b.Publisher)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book.BookAuthors)
           .ThenInclude(ba => ba.Author)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book.BookGenres)
           .ThenInclude(bg => bg.Genre)
           .FirstOrDefault(m => m.Id == id);

            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }
        public async Task<IActionResult> Details_Admin(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = _context.Shelves
           .Include(s => s.User)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book)
           .ThenInclude(b => b.Publisher)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book.BookAuthors)
           .ThenInclude(ba => ba.Author)
           .Include(s => s.ShelfBooks)
           .ThenInclude(sb => sb.Book.BookGenres)
           .ThenInclude(bg => bg.Genre)
           .FirstOrDefault(m => m.Id == id);

            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }
        // GET: Shelves/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: Shelves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UserId,IsPrivate,Id")] Shelf shelf)
        {
            ModelState.Remove("User");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                
                shelf.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(shelf);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index_Admin");
        }
        else if (User.IsInRole("User"))
        {
            return RedirectToAction("Index_User");
        }
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", shelf.UserId);
            return View(shelf);
        }

        [HttpGet]
        public IActionResult AddBook(long shelfId)
        {
            var shelf = _context.Shelves
                .Include(s => s.ShelfBooks)
                .FirstOrDefault(s => s.Id == shelfId);

            if (shelf == null)
            {
                return NotFound();
            }

            var booksInShelf = shelf.ShelfBooks.Select(sb => sb.BookId).ToList();
            var availableBooks = _context.Books
                .Where(b => !booksInShelf.Contains(b.Id))
                .Include(b => b.Publisher)
                .ToList();

            ViewBag.AvailableBooks = availableBooks;
            return View(shelfId);
        }

        [HttpPost]
        public IActionResult AddBook(long shelfId, long bookId, string? comment)
        {
            var shelf = _context.Shelves.Find(shelfId);
            var book = _context.Books.Find(bookId);

            if (shelf == null || book == null)
            {
                return NotFound();
            }

            var shelfBook = new ShelfBook
            {
                ShelfId = shelfId,
                BookId = bookId,
                Comment = comment
            };

            _context.ShelfBooks.Add(shelfBook);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = shelfId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromShelf(long shelfId, long bookId)
        {
            var shelfBook = await _context.ShelfBooks
                .FirstOrDefaultAsync(sb => sb.ShelfId == shelfId && sb.BookId == bookId);

            if (shelfBook == null)
            {
                return NotFound();
            }

            _context.ShelfBooks.Remove(shelfBook);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details_User", new { id = shelfId });
        }

        // GET: Shelves/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelves.FindAsync(id);
            if (shelf == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", shelf.UserId);
            return View(shelf);
        }

        // POST: Shelves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,UserId,IsPrivate,Id")] Shelf shelf)
        {
            if (id != shelf.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shelf);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShelfExists(shelf.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", shelf.UserId);
            return View(shelf);
        }

        // GET: Shelves/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelf = await _context.Shelves
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shelf == null)
            {
                return NotFound();
            }

            return View(shelf);
        }

        // POST: Shelves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var shelf = await _context.Shelves.FindAsync(id);
            if (shelf != null)
            {
                var shelfBooks = _context.ShelfBooks.Where(sb => sb.ShelfId == id);
                _context.ShelfBooks.RemoveRange(shelfBooks);
                _context.Shelves.Remove(shelf);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShelfExists(long id)
        {
            return _context.Shelves.Any(e => e.Id == id);
        }
    }
}