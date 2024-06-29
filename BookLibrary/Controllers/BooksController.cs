using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Controllers
{
    /// <summary>
    /// Controller for handling book-related operations.
    /// </summary>
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookController"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the controller.</param>
        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the list of books with optional search functionality.
        /// </summary>
        /// <param name="searchString">The search query to filter books.</param>
        /// <returns>A view that displays the list of books.</returns>
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParm"] = sortOrder == "author" ? "author_desc" : "author";
            ViewData["CurrentFilter"] = searchString;

            var books = from b in _context.Books
                        select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                case "author":
                    books = books.OrderBy(b => b.Author);
                    break;
                case "author_desc":
                    books = books.OrderByDescending(b => b.Author);
                    break;
                default:
                    books = books.OrderBy(b => b.Title);
                    break;
            }

            return View(await books.ToListAsync());
        }

        /// <summary>
        /// Displays details of a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to be displayed.</param>
        /// <returns>A view that displays the details of the book.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        /// <summary>
        /// Displays the create book view.
        /// </summary>
        /// <returns>A view that allows the user to create a new book.</returns>
        public IActionResult Create()
        {
            return View();

        }

        /// <summary>
        /// Handles the POST request for creating a new book.
        /// </summary>
        /// <param name="book">The book to be created.</param>
        /// <returns>A view that displays the newly created book or the create view if model state is invalid.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,Year,ISBN,Pages")] Book book)
        {
            if (ModelState.IsValid)
            {
                // Check if an identical book already exists
                var existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.Title == book.Title && b.Author == book.Author && b.Year == book.Year && b.ISBN == book.ISBN && b.Pages == book.Pages);

                if (existingBook != null)
                {
                    // Book already exists, return an error message or handle as needed
                    ModelState.AddModelError(string.Empty, "A book with the same title, author, year, ISBN, and pages already exists.");
                    return View(book);
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        /// <summary>
        /// Displays the edit book view for a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to be edited.</param>
        /// <returns>A view that allows the user to edit the book.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        /// <summary>
        /// Handles the POST request for editing an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to be edited.</param>
        /// <param name="book">The updated book information.</param>
        /// <returns>A view that displays the updated book or the edit view if model state is invalid.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Year,ISBN,Pages")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        /// <summary>
        /// Displays the delete book view for a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to be deleted.</param>
        /// <returns>A view that allows the user to delete the book.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        /// <summary>
        /// Handles the POST request for confirming the deletion of a book.
        /// </summary>
        /// <param name="id">The ID of the book to be deleted.</param>
        /// <returns>A redirect to the index view if the deletion is successful.</returns>
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a book exists in the database.
        /// </summary>
        /// <param name="id">The ID of the book to check.</param>
        /// <returns>True if the book exists, false otherwise.</returns>
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
