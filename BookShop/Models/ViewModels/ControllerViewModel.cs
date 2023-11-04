namespace BookShop.Models.ViewModels;

public class ControllerViewModel
{
    public string AreaName { get; set; }
    public IList<Attribute> ControllerAttributes { get; set; }
    public string ControllerDisplayName { get; set; }
    public string ControllerName { get; set; }
    public string ControllerId { get; set; }
    public IList<ActionViewModel> Actions { get; set; } = new List<ActionViewModel>();
}
