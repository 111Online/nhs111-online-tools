using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHS111.Online.Tools.Models;
using NHS111.Online.Tools.Models.ManageViewModels;
using NHS111.Online.Tools.Models.Security;

namespace NHS111.Online.Tools.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ManageController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public string StatusMessage { get; set; }

        public ActionResult ViewUser()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListUsers()
        {
            var users = _userManager.Users.ToArray();
            var model = users.Select(user => new EditUserViewModel()
                {
                    Email = user.Email,
                    Status = Enum.GetName(typeof(RegistrationStatus), user.Status)
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> EditUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with email '{email}'.");
            }

            var model = new EditUserViewModel()
            {
                Email = user.Email,
                Status = Enum.GetName(typeof(RegistrationStatus), user.Status)
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with Email '{model.Email}'.");
            }

            StatusMessage = "Your profile has been updated";
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

            return RedirectToAction(nameof(ChangePassword));
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

        #endregion
    }
}
