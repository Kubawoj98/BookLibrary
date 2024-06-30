using Microsoft.AspNetCore.Mvc;
using BookLibrary.Data;
using BookLibrary.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Controllers
{
    /// <summary>
    /// Controller responsible for handling user authentication and registration.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the registration view.
        /// </summary>
        /// <returns>The registration view.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles the registration of a new user.
        /// </summary>
        /// <param name="model">The registration view model containing user details.</param>
        /// <returns>
        /// Redirects to the login view if registration is successful; otherwise, returns the registration view with validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if an identical user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "User already exist.");
                    return View(model);
                }
                var user = new User
                {
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "Regular" // Assign default role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Register", "Auth");
            }
            return View(model);
        }

        /// <summary>
        /// Displays the login view.
        /// </summary>
        /// <returns>The login view.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <param name="model">The login view model containing user credentials.</param>
        /// <returns>
        /// Redirects to the book index view if login is successful; otherwise, returns the login view with validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                // Check if user exists
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // Set session or authentication cookie here if needed
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            return View(model);
        }
    }
}
