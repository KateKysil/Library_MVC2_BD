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
    public class BookGenresController : Controller
    {
        private readonly LibraryContext _context;

        public BookGenresController(LibraryContext context)
        {
            _context = context;
        }

        // GET: BookGenres
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.BookGenres.Include(b => b.Book).Include(b => b.Genre);
            return View(await libraryContext.ToListAsync());
        }

        // GET: BookGenres/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenre = await _context.BookGenres
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookGenre == null)
            {
                return NotFound();
            }

            return View(bookGenre);
        }

        // GET: BookGenres/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName");
            return View();
        }

        // POST: BookGenres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,GenreId,Id")] BookGenre bookGenre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookGenre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn", bookGenre.BookId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", bookGenre.GenreId);
            return View(bookGenre);
        }

        // GET: BookGenres/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenre = await _context.BookGenres.FindAsync(id);
            if (bookGenre == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn", bookGenre.BookId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", bookGenre.GenreId);
            return View(bookGenre);
        }

        // POST: BookGenres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("BookId,GenreId,Id")] BookGenre bookGenre)
        {
            if (id != bookGenre.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookGenre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookGenreExists(bookGenre.BookId))
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
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Isbn", bookGenre.BookId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", bookGenre.GenreId);
            return View(bookGenre);
        }

        // GET: BookGenres/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenre = await _context.BookGenres
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookGenre == null)
            {
                return NotFound();
            }

            return View(bookGenre);
        }

        // POST: BookGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var bookGenre = await _context.BookGenres.FindAsync(id);
            if (bookGenre != null)
            {
                _context.BookGenres.Remove(bookGenre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookGenreExists(long id)
        {
            return _context.BookGenres.Any(e => e.BookId == id);
        }
    }
}
