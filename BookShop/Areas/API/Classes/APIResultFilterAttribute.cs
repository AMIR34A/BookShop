using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookShop.Areas.API.Classes;

public class APIResultFilterAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.Result = context.Result switch
        {
            OkObjectResult okObjectResult => new JsonResult(new APIResult<object>(true, ApiResultStatusCode.Success, okObjectResult.Value)) { StatusCode = okObjectResult.StatusCode },
            OkResult okResult => new JsonResult(new APIResult(true, ApiResultStatusCode.Success)) { StatusCode = okResult.StatusCode },
            BadRequestResult badRequestResult => new JsonResult(new APIResult(false, ApiResultStatusCode.BadRequest)) { StatusCode = badRequestResult.StatusCode },
            BadRequestObjectResult badRequestObjectResult => CreateBadRequestObjectResult(badRequestObjectResult),
            ContentResult contentResult => new JsonResult(new APIResult(true, ApiResultStatusCode.Success, new List<string> { contentResult.Content })) { StatusCode = contentResult.StatusCode },
            NotFoundResult notFoundResult => new JsonResult(new APIResult(false, ApiResultStatusCode.NotFound)) { StatusCode = notFoundResult.StatusCode },
            NotFoundObjectResult notFoundObjectResult => new JsonResult(new APIResult<object>(false, ApiResultStatusCode.NotFound, notFoundObjectResult.Value)) { StatusCode = notFoundObjectResult.StatusCode },
            ObjectResult objectResult => new JsonResult(new APIResult<object>(true, ApiResultStatusCode.Success, objectResult.Value)) { StatusCode = objectResult.StatusCode },
            _ => new JsonResult(new APIResult(true, ApiResultStatusCode.ListEmpty))
        };
        base.OnResultExecuting(context);
    }

    private JsonResult CreateBadRequestObjectResult(BadRequestObjectResult badRequestObjectResult)
    {
        List<string> messages = (new List<string>());
        if (badRequestObjectResult.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            messages.AddRange(errorMessages);
        }
        else
            messages.Add(badRequestObjectResult.Value.ToString());

        return new JsonResult(new APIResult(false, ApiResultStatusCode.BadRequest, messages)) { StatusCode = badRequestObjectResult.StatusCode };
    }
}
