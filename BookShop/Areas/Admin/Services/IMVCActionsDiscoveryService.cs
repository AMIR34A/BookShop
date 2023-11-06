using BookShop.Models.ViewModels;

namespace BookShop.Areas.Admin.Services;

public interface IMVCActionsDiscoveryService
{
    ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
}
