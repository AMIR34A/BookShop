using BookShop.Areas.API.Classes;
using BookShop.Exceptions;
using Newtonsoft.Json;
using System.Net;


namespace BookShop.Areas.API.Middlewares;

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder CustomExceptionHandler(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment)
    {
        _next = next;
        _webHostEnvironment = webHostEnvironment;
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
        catch (AppException exception)
        {
            httpStatusCode = exception.HttpStatusCode;
            apiResultStatusCode = exception.ApiStatusCode;

            if (_webHostEnvironment.IsDevelopment())
            {
                var error = new Dictionary<string, string>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };

                if (exception.InnerException is not null)
                {
                    error.Add("InnerException.Exception", exception.InnerException.Message);
                    error.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                }
                if (exception.AdditionalData is not null)
                    error.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                messages.Add(JsonConvert.SerializeObject(error));
            }
            else
                messages.Add("Something went wrong.");

            await WriteToResponseAsync();
        }
        catch (Exception exception)
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                var error = new Dictionary<string, string>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace
                };

                messages.Add(JsonConvert.SerializeObject(error));
            }
            else
                messages.Add("Something went wrong.");

            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            var result = new APIResult(false, apiResultStatusCode, messages);
            var jsonResult = JsonConvert.SerializeObject(result);

            httpContext.Response.StatusCode = (int)httpStatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(jsonResult);
        }
    }
}
