using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.ViewModels;

public class RegisterViewModel : GoogleRecaptchaModelBase
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
    public string Username { get; set; }

    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "کلمه عبور")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "تکرار کلمه عبور")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "شماره موبایل")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    public string PhoneNumber { get; set; }
}

public class SignInViewModel
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    public string Username { get; set; }

    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "مرا بخاطر بسپار")]
    public bool RememberMe { get; set; }

    [Display(Name = "کد امنیتی")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(4, ErrorMessage = "کد امنیتی باید شامل 4 کاراکتر باشید.")]
    public string CaptchaCode { get; set; }
}

public class ForgetPasswordViewModel
{
    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
    public string Email { get; set; }
}

public class ResetPasswordViewModel
{
    [Display(Name = "ایمیل")]
    public string Email { get; set; }

    [Display(Name = "رمز عبور")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
    public string Password { get; set; }

    [Display(Name = "تکرار رمز عبور")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده مطابقت ندارد.")]
    public string ConfirmPassword { get; set; }
    public string Token { get; set; }
}
public class SendCodeViewModel
{
    public string SelectedProvider { get; set; }
    public ICollection<SelectListItem>? Providers { get; set; }
    public bool RememberMe { get; set; }
}

public class VerifyCodeViewModel
{
    [Required]
    public string Provider { get; set; }

    [Display(Name = "کد اعتبارسنجی")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    public string Code { get; set; }

    [Display(Name = "مرا بخاطر بسپار؟")]
    public bool RememberBrowser { get; set; }

    [Display(Name = "RememberMe?")]
    public bool RememberMe { get; set; }
}

public class ChangePasswordViewModel
{
    [DataType(DataType.Password)]
    [Display(Name ="رمز عبور قدیمی")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name ="رمز عبور جدید")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
    public string NewPassword { get; set; }

    [Display(Name ="تکرار رمز عبور")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [Compare("NewPassword", ErrorMessage = "تکرار کلمه عبور با کلمه عبور جدید وارد شده مطابقت ندارد.")]
    public string ConfirmPassword { get; set; }

    public UserSidebarViewModel UserSidebar { get; set; }
}

public class UserSidebarViewModel
{
    public string FullName { get; set; }
    public DateTime LastVisit { get; set; }
    public DateTime RegisterTime { get; set; }
    public string Image { get; set; }
}
