using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersAPIController : ControllerBase
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IApplicationRoleManager _applicationRoleManager;

    public UsersAPIController(IApplicationUserManager applicationUserManager, IApplicationRoleManager applicationRoleManager)
    {
        _applicationUserManager = applicationUserManager;
        _applicationRoleManager = applicationRoleManager;
    }
}
