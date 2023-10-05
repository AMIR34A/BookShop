using BookShop.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Models.ViewModels;

public class GoogleRecaptchaModelBase
{
    [GoogleRecaptchaValidation]
    [BindProperty(Name = "g-recaptcha-response")]
    public string GoogleRecaptchaResponse { get; set; }
}
