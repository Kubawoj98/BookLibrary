using Microsoft.AspNetCore.Mvc;
using BookLibrary.Data;
using BookLibrary.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Auth");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // Możesz tutaj ustawić sesję lub plik cookie dla zalogowanego użytkownika
                    return RedirectToAction("Index", "Book");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }
    }
}
