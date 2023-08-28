// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IApplicationRoleManager _roleManager;
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<RegisterModel> logger,
            IApplicationRoleManager roleManager)

        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public IEnumerable<ApplicationRole> Roles { get; set; }

        public class InputModel
        {

            [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
            [EmailAddress(ErrorMessage = "ایمیل شما معتبر نیست")]
            [Display(Name = "ایمیل")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} باید شمامل حداقل {1} و شامل {2} حرف و عدد باشد", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "کلمه عبور")]
            public string Password { get; set; }


            [DataType(DataType.Password)]
            [Display(Name = "تکرار کلمه عبور")]
            [Compare("Password", ErrorMessage = "با کلمه عبور مطابقت ندراد")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "نام")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
            public string FirstName { get; set; }

            [Display(Name = "نام خانوادگی")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
            public string LastName { get; set; }

            [Display(Name = "نام کاربری")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
            public string Username { get; set; }

            [Display(Name = "شماره همراه")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
            public string PhoneNumber { get; set; }

            [Display(Name = "تاریخ تولد")]
            [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
            public string BirthDate { get; set; }

            public string[] UserRoles { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Roles = _roleManager.GetAllRoles();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Admin/UsersManager/Index?message=success");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserName = Input.Username,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    BirthDate = DateTime.Parse(Input.BirthDate),
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, Input.UserRoles);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            Roles = _roleManager.GetAllRoles();
            return Page();
        }
    }
}
