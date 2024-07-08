﻿using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
[ApiExplorerSettings(IgnoreApi = true)]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
