using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Attributes;

public class GoogleRecaptchaValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        Lazy<ValidationResult> validationResult = new Lazy<ValidationResult>(() => new ValidationResult("لطفا با زدن تیک من ربات نیستم هویت خود را تایید کنید.", new String[] { validationContext.MemberName }));
        if (value is null)
            return validationResult.Value;

        IConfiguration configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
        string reCaptchaResponse = value.ToString();
        string reCaptchaSecret = configuration.GetValue<string>("GoogleRecaptcha:SecretKey");

        using HttpClient httpClient = new HttpClient();
        var httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchaResponse}").Result;
        if (!httpResponse.IsSuccessStatusCode)
            return validationResult.Value;
        var jesonResponseAsString = httpResponse.Content.ReadAsStringAsync().Result;
        var jsonData = JObject.Parse(jesonResponseAsString);
        if (jsonData.Value<bool>("success") != true)
            return validationResult.Value;
        return ValidationResult.Success;
    }
}
