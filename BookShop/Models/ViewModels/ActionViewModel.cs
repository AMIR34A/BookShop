namespace BookShop.Models.ViewModels;

public class ActionViewModel
{
    public IList<Attribute> ActionAttributes { get; set; }
    public string ActionDisplayName { get; set; }
    public string ControllerId { get; set; }
    public string ActionName { get; set; }
    public string ActionId => $"{ControllerId}:{ActionName}";
    public bool IsSecuredAction { get; set; }
}