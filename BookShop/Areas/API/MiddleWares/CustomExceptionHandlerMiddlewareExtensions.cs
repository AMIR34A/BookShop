using BookShop.Areas.API.Classes;
using Newtonsoft.Json;
using System.Net;

namespace BookShop.Areas.API.Middlewares;

public class CustomExceptionHandlerMiddlewareExtensions
{
}

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        List<string> messages = new List<string>();
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        ApiResultStatusCode apiResultStatusCode = ApiResultStatusCode.ServerError;

        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var error = new Dictionary<string, string>
            {
                ["Exception"] = exception.Message,
                ["StackTrace"] = exception.StackTrace
            };

            messages.Add(JsonConvert.SerializeObject(error));
            var result = new APIResult(false, apiResultStatusCode, messages);
            var jsonResult = JsonConvert.SerializeObject(result);

            httpContext.Response.StatusCode = (int)httpStatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(jsonResult);
        }
    }
}
