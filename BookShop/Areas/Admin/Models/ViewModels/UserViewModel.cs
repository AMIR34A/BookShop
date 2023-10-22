using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.Admin.Models.ViewModels;

public class EnableAuthenticatorViewModel
{
    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    [StringLength(7, ErrorMessage = "کد اعتبارسنجی باید حداقل دارای {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "کد اعتبارسنجی")]
    public string Code { get; set; }
    public string SharedKey { get; set; }
    public string AuthenticatorUri { get; set; }
}

public class TwoFactorAuthenticationViewModel
{
    public bool HasAuthenticator { get; set; }
    public int RecoveryCodesLeft { get; set; }
    public bool Is2FAEnabled { get; set; }
}