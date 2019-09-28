using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHS111.Online.Tools.Models;
using NHS111.Online.Tools.Models.RoleViewModels;

namespace NHS111.Online.Tools.Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger _logger;

        public RoleController(RoleManager<ApplicationRole> roleManager, ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult List()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("", $"The role {model.Name} already exists!");
                return View(model);
            }

            var role = new ApplicationRole(model.Name);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Role {role} was successfully created!");
                return RedirectToAction("List", "Role");
            }

            AddErrors(result);
            return View(model);
        }

        public async Task<ActionResult> Delete(DeleteRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Role {role} was successfully deleted!");
                return RedirectToAction("List", "Role");
            }

            AddErrors(result);
            return View(model);
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