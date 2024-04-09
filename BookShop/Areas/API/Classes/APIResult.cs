using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookShop.Areas.API.Classes;

public class APIResult
{
    public bool IsSuccess { get; set; }
    public ApiResultStatusCode StatusCode { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Message { get; set; }

    public APIResult(bool isSuccess, ApiResultStatusCode statusCode, List<string>? message = null)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Message = message ?? statusCode.ToDisplay();
    }

    #region Implicit Operators
    public static implicit operator APIResult(OkResult result) => new APIResult(true, ApiResultStatusCode.Success);

    public static implicit operator APIResult(BadRequestResult result) => new APIResult(false, ApiResultStatusCode.BadRequest);

    public static implicit operator APIResult(BadRequestObjectResult result)
    {
        List<string> messages = new List<string>();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            messages.AddRange(errorMessages);
        }

        else if (result.Value is IEnumerable<IdentityError> identityErrors)
        {
            var errorMessages = identityErrors.Select(p => p.Description).Distinct();
            messages.AddRange(errorMessages);
        }

        else
            messages.Add(result.Value.ToString());

        return new APIResult(false, ApiResultStatusCode.BadRequest, messages);
    }

    public static implicit operator APIResult(ContentResult result)
    {
        List<string> messages = new List<string>() { result.Content };
        return new APIResult(true, ApiResultStatusCode.Success, messages);
    }

    public static implicit operator APIResult(NotFoundResult result) => new APIResult(false, ApiResultStatusCode.NotFound);
    #endregion
}

public class APIResult<TData> : APIResult where TData : class
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TData Data { get; set; }

    public APIResult(bool isSuccess, ApiResultStatusCode statusCode, TData data, List<string>? message = null) : base(isSuccess, statusCode, message)
    {
        Data = data;
    }

    #region Implicit Operators
    public static implicit operator APIResult<TData>(TData data) => new APIResult<TData>(true, ApiResultStatusCode.Success, data);

    public static implicit operator APIResult<TData>(OkResult result) => new APIResult<TData>(true, ApiResultStatusCode.Success, null);

    public static implicit operator APIResult<TData>(OkObjectResult result) => new APIResult<TData>(true, ApiResultStatusCode.Success, (TData)result.Value);

    public static implicit operator APIResult<TData>(BadRequestResult result) => new APIResult<TData>(false, ApiResultStatusCode.BadRequest, null);

    public static implicit operator APIResult<TData>(BadRequestObjectResult result)
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

        return new APIResult<TData>(false, ApiResultStatusCode.BadRequest, null, message);
    }

    public static implicit operator APIResult<TData>(ContentResult result)
    {
        List<string> messages = new List<string>() { result.Content };
        return new APIResult<TData>(true, ApiResultStatusCode.Success, null, messages);
    }

    public static implicit operator APIResult<TData>(NotFoundResult result) => new APIResult<TData>(false, ApiResultStatusCode.NotFound, null);

    public static implicit operator APIResult<TData>(NotFoundObjectResult result) => new APIResult<TData>(false, ApiResultStatusCode.NotFound, (TData)result.Value);
    #endregion
}