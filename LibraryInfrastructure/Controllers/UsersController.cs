using DocumentFormat.OpenXml.Spreadsheet;
using LibraryDomain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInfrastructure.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList(); // IQueryable<User>
            return View(users); // Passes users to Index.cshtml
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }
        

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details_Admin(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user); 
        }
        public async Task<IActionResult> Details_User(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        public async Task<IActionResult> Edit_Admin(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Users/Edit_Admin/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_Admin(string id, User updatedUser)
        {
            if (id != updatedUser.Id) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.UserName = updatedUser.UserName;
                user.Birthday = updatedUser.Birthday;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Details_Admin), new { id = user.Id });

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(updatedUser);
        }

    }
}
