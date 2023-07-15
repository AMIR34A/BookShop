using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.ViewModels
{
    public class TranslatorsCreateViewModel
    {
        public int TranslatorId { get; set; }
        [Display(Name = "First Name"), Required(AllowEmptyStrings = false, ErrorMessage = "You should enter {0}")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name"), Required(AllowEmptyStrings = false, ErrorMessage = "You should enter {0}")]
        public string LastName { get; set; }
    }
}
