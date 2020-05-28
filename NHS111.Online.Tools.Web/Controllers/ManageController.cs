using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHS111.Online.Tools.Models.Identity;
using NHS111.Online.Tools.Models.ManageViewModels;
using NHS111.Online.Tools.Models.SharedViewModels;

namespace NHS111.Online.Tools.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger _logger;

        public string StatusMessage { get; set; }

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<ManageController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public ActionResult ViewUser()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListUsers()
        {
            var users = _userManager
                .Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToList();

            var model = users.Select(user => new UserViewModel()
                {
                    Email = user.Email,
                    SelectedRoles = user.UserRoles.Select(r => r.Role.Name)
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> EditUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with email '{email}'.");
            }

            var model = new EditUserViewModel()
            {
                Email = user.Email,
                SelectedRoles = roles,
                Roles = _roleManager.Roles.Select(r => new SelectListItem(r.Name, r.Name))
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var unselectedRoles = _roleManager
                    .Roles
                    .Where(r => model.SelectedRoles.All(sr => sr != r.Name))
                    .Select(r => r.Name);

                await RemoveFromRoles(user, unselectedRoles);
                await AddToRoles(user, model.SelectedRoles);

                _logger.LogInformation($"Profile for user {user.Email} has been updated");
                return RedirectToAction("ListUsers", "Manage");
            }

            _logger.LogInformation($"Profile for user {user.Email} failed");
            model.Roles = _roleManager.Roles.Select(r => new SelectListItem(r.Name, r.Name));
            return RedirectToAction(nameof(ListUsers));
        }

        [Authorize]
        public async Task<ActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with email '{email}'.");
            }

            var deleteUserResult = await _userManager.DeleteAsync(user);
            if (!deleteUserResult.Succeeded)
            {
                throw new ApplicationException($"Unable to delete user with email '{email}'.");
            }

            return RedirectToAction(nameof(ListUsers));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ViewUser));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task AddToRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                    await _userManager.AddToRoleAsync(user, role);
            }
        }

        private async Task RemoveFromRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role))
                    await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        #endregion
    }
}
