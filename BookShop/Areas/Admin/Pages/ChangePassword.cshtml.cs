using BookShop.Areas.Admin.Models;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShop.Areas.Admin.Pages;

public class ChangePasswordModel : PageModel
{
    private readonly IApplicationUserManager _userManager;

    public ChangePasswordModel(IApplicationUserManager userManager) => _userManager = userManager;

    [BindProperty]
    public ChangePasswordInputModel ChangePasswordInputModel { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();

        if (ModelState.IsValid)
        {
            IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, ChangePasswordInputModel.OldPassword, ChangePasswordInputModel.NewPassword);
            if (identityResult.Succeeded)
                ViewData.Add("Alert", "Alert");
            else
            {
                foreach (var error in identityResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return Page();
    }
}
