using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.API.Models;

public enum ApiResultStatusCode
{
    [Display(Name = "عملیات با موفقیت انجام شد")]
    Success,

    [Display(Name = "خطایی در سرور رخ داده است")]
    ServerError,

    [Display(Name = "پارامتر های ارسالی معتبر نیستند")]
    BadRequest,

    [Display(Name = "یافت نشد")]
    NotFound,

    [Display(Name = "لیست خالی است")]
    ListEmpty,

    [Display(Name = "خطایی در پردازش رخ داد")]
    LogicError,

    [Display(Name = "خطای احراز هویت")]
    UnAuthorized
}
