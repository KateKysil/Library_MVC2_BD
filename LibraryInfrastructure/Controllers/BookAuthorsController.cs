using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Model;
using LibraryInfrastructure;

namespace LibraryInfrastructure.Controllers
{
    public class BookAuthorsController : Controller
    {
        private readonly LibraryContext _context;

        public BookAuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: BookAuthors
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.BookAuthors.Include(b => b.Author).Include(b => b.Book);
            return View(await libraryContext.ToListAsync());
        }

        // GET: BookAuthors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthor = await _context.BookAuthors
                .Include(b => b.Author)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookAuthor == null)
            {
                return NotFound();
            }

            return View(bookAuthor);
        }

        // GET: BookAuthors/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Country");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn");
            return View();
        }

        // POST: BookAuthors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,AuthorId,Id")] BookAuthor bookAuthor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookAuthor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Country", bookAuthor.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn", bookAuthor.BookId);
            return View(bookAuthor);
        }

        // GET: BookAuthors/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthor = await _context.BookAuthors.FindAsync(id);
            if (bookAuthor == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Country", bookAuthor.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn", bookAuthor.BookId);
            return View(bookAuthor);
        }

        // POST: BookAuthors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("BookId,AuthorId,Id")] BookAuthor bookAuthor)
        {
            if (id != bookAuthor.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookAuthor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookAuthorExists(bookAuthor.BookId))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Country", bookAuthor.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn", bookAuthor.BookId);
            return View(bookAuthor);
        }

        // GET: BookAuthors/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthor = await _context.BookAuthors
                .Include(b => b.Author)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookAuthor == null)
            {
                return NotFound();
            }

            return View(bookAuthor);
        }

        // POST: BookAuthors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var bookAuthor = await _context.BookAuthors.FindAsync(id);
            if (bookAuthor != null)
            {
                _context.BookAuthors.Remove(bookAuthor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookAuthorExists(long id)
        {
            return _context.BookAuthors.Any(e => e.BookId == id);
        }
    }
}
