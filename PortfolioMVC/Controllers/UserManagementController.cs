using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Models.entities;
using PortfolioMVC.Models.ViewModels;

namespace PortfolioMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Department = user.Department,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // GET: UserManagement/Details/5
        public async Task<IActionResult> Details(string id)
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

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }

        // GET: UserManagement/Edit/5
        public async Task<IActionResult> Edit(string id)
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

            var roles = await _userManager.GetRolesAsync(user);
            var availableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                UserRoles = roles.ToList(),
                AllRoles = availableRoles
            };

            return View(viewModel);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Prevent modifying the last admin
                if (await IsLastAdminAsync(user) && !model.UserRoles.Contains("Admin"))
                {
                    ModelState.AddModelError("", "Cannot remove the last admin user.");
                    model.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }

                // Update user details
                user.Name = model.Name;
                user.Department = model.Department;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    model.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }

                // Get current roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Remove roles that are no longer selected
                foreach (var role in userRoles)
                {
                    if (!model.UserRoles.Contains(role))
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, role);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                // Add newly selected roles
                foreach (var role in model.UserRoles)
                {
                    if (!userRoles.Contains(role))
                    {
                        result = await _userManager.AddToRoleAsync(user, role);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            model.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }

        // GET: UserManagement/Delete/5
        public async Task<IActionResult> Delete(string id)
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

            // Prevent deleting the current user
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent deleting the last admin
            if (await IsLastAdminAsync(user))
            {
                TempData["ErrorMessage"] = "Cannot delete the last admin user.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }

        // POST: UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting the current user
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent deleting the last admin
            if (await IsLastAdminAsync(user))
            {
                TempData["ErrorMessage"] = "Cannot delete the last admin user.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsLastAdminAsync(AppUser user)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.Count == 1 && admins.Contains(user);
        }
    }
}