using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.Admin.Models;

public class ChangePasswordModel
{
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور قدیمی")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور جدید")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
    public string NewPassword { get; set; }

    [Display(Name = "تکرار رمز عبور")]
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [Compare("NewPassword", ErrorMessage = "تکرار کلمه عبور با کلمه عبور جدید وارد شده مطابقت ندارد.")]
    public string ConfirmPassword { get; set; }
    public AdminSidebarModel AdminSidebar { get; set; }
}
