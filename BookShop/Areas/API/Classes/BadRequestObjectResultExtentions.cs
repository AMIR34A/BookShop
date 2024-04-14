using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Classes;

public static class BadRequestObjectResultExtentions
{
    public static List<string> GetErrors(this BadRequestObjectResult result)
    {
        List<string> message = new List<string>();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message.AddRange(errorMessages);
        }
        else if (result.Value is IEnumerable<IdentityError> identityErrors)
        {
            var errorMessages = identityErrors.Select(p => p.Description).Distinct();
            message.AddRange(errorMessages);
        }
        else
            message.Add(result.Value.ToString());
        return message;
    }
}