using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;

namespace BookShop.Areas.Admin.Models.ViewModels;

public class DynamicAccessIndexViewModel
{
    public string RoleId { get; set; }
    public string[] ActionsId { get; set; }
    public ApplicationRole RoleWithClaims { get; set; }
    public ICollection<ControllerViewModel> SecuredControllerActions { get; set; }
}
