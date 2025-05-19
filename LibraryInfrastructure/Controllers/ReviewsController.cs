using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Model;
using LibraryInfrastructure;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;

namespace LibraryInfrastructure.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly LibraryContext _context;

        public ReviewsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(int? id, string? name, string? type)
        {
            if (type == "books")
            {
                if (id == null) return RedirectToAction("Books", "Index");
                ViewBag.Id = id;
                var CommentsByBook = _context.Reviews.Where(r => r.BookId == id).Include(b => b.Book).Include(b => b.User);
                return View(await CommentsByBook.ToListAsync());
            }
            var libraryContext = _context.Reviews.Include(r => r.Book).Include(r => r.User);
            return View(await libraryContext.ToListAsync());
        }
        public async Task<IActionResult> Index_Admin(int? id, string? name, string? type)
        {
            if (type == "books")
            {
                if (id == null) return RedirectToAction("Books", "Index_Admin");
                ViewBag.Id = id;
                ViewBag.Name = "до " + name;
                var CommentsByBook = _context.Reviews.Where(r => r.BookId == id).Include(b => b.Book).Include(b => b.User);
                return View(await CommentsByBook.ToListAsync());
            }
            var libraryContext = _context.Reviews.Include(r => r.Book).Include(r => r.User);
            return View(await libraryContext.ToListAsync());
        }
        public async Task<IActionResult> Index_User(int? id, string? name, string? type)
        {
            if (type == "books")
            {
                if (id == null) return RedirectToAction("Books", "Index_User");
                ViewBag.Id = id;
                ViewBag.Name = "до " + name;
                var CommentsByBook = _context.Reviews.Where(r => r.BookId == id).Include(b => b.Book).Include(b => b.User);
                return View(await CommentsByBook.ToListAsync());
            }
            var libraryContext = _context.Reviews.Include(r => r.Book).Include(r => r.User);
            return View(await libraryContext.ToListAsync());
        }
        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }



        // GET: Reviews/Create
        public IActionResult Create(int? id)
        {
            var books = _context.Books.ToList();
            ViewData["BookId"] = new SelectList(books, "Id", "Title", id);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewData["BookTitle"] = _context.Books.FirstOrDefault(b => b.Id == id).Title;
            return View();
        }
        public IActionResult Create_Admin(int? id)
        {
            var books = _context.Books.ToList();
            ViewData["BookId"] = new SelectList(books, "Id", "Title", id);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewData["BookTitle"] = _context.Books.FirstOrDefault(b => b.Id == id).Title;
            return View();
        }
        public IActionResult Create_User(int? id)
        {
            var books = _context.Books.ToList();
            ViewData["BookId"] = new SelectList(books, "Id", "Title", id);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
            ViewData["BookTitle"] = _context.Books.FirstOrDefault(b => b.Id == id).Title;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,UserId,Text,Rate")] Review review)
        {
            ModelState.Remove("Book");
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
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(review);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Your comment saved!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index", "Books");
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", review.UserId);
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Admin([Bind("BookId,UserId,Text,Rate")] Review review)
        {
            ModelState.Remove("Book");
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
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(review);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Your comment saved!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index_Admin", "Books", new { id = review.BookId, name = review.Book.Title, type = "books" });
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", review.UserId);
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_User([Bind("BookId,UserId,Text,Rate")] Review review)
        {
            ModelState.Remove("Book");
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
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(review);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Your comment saved!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index_User", "Reviews", new { id = review.BookId, name = review.Book.Title, type = "books" });
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", review.UserId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", review.UserId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("BookId,UserId,Text,Rate")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", review.UserId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(long id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}