using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.Admin.Models.ViewModels;

public class RolesViewModel
{
    public string RoleId { get; set; }
    [Display(Name = "عنوان نقش")]
    public string RoleName { get; set; }
    [Display(Name = "توضیحات")]
    public string Description { get; set; }
    [Display(Name = "تعداد کاربران")]
    public int UserCount { get; set; }
}