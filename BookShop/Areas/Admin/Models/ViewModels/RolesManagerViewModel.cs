using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.Admin.Models.ViewModels;

public class RolesViewModel
{
    public string RoleId { get; set; }
    [Display(Name ="عنوان نقش")]
    public string RoleName { get; set; }
}