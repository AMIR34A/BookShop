using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.ViewModels;

public class AccountViewModel
{
    [Display(Name ="نام کاربری")]
    [Required(ErrorMessage ="وارد نمودن {0} الزامی است")]
    public string Username { get; set; }

    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "کلمه عبور")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
    public string  Password { get; set; }

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

    [Display(Name ="کد امنیتی")]
    [Required(ErrorMessage = ".وارد نمودن {0} الزامی است")]
    [StringLength(4, ErrorMessage = "کد امنیتی باید شامل 4 کاراکتر باشید.")]
    public string CaptchaCode { get; set; }
}
