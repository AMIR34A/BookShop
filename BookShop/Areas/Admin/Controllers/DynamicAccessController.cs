using BookShop.Areas.Admin.Data;
using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Areas.Admin.Services;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DynamicAccessController : Controller
    {
        private readonly IApplicationRoleManager _roleManager;
        private readonly IMVCActionsDiscoveryService _actionsDiscoveryService;

        public DynamicAccessController(IApplicationRoleManager roleManager, IMVCActionsDiscoveryService actionsDiscoveryService)
        {
            _roleManager = roleManager;
            _actionsDiscoveryService = actionsDiscoveryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var role = await _roleManager.FindClaimsInRolesAsync(id);
            if (role is null)
                return NotFound();

            DynamicAccessIndexViewModel dynamicAccessViewModel = new DynamicAccessIndexViewModel
            {
                RoleWithClaims = role,
                SecuredControllerActions = _actionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamicPermission)
            };

            return View(dynamicAccessViewModel);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DynamicAccessIndexViewModel dynamicAccessIndexViewModel)
        {
            IdentityResult identityResult = await _roleManager.AddOrUpdateClaimsAsync(dynamicAccessIndexViewModel.RoleId, ConstantPolicies.DynamicPermission, dynamicAccessIndexViewModel.ActionsId);
            if (!identityResult.Succeeded)
                ModelState.AddModelError(string.Empty, "در حین انجام عملیات خطایی رخ داد.");
            return RedirectToAction("Index", new { id = dynamicAccessIndexViewModel.RoleId });
        }
    }
}