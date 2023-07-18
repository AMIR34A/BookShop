using BookShop.Models.Repository;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class ProvincesController : Controller
{
    IUnitOfWork unitOfWork;
    public ProvincesController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var provinces = await unitOfWork.RepositoryBase<Province>().GetAllAsync();
        return View(provinces);
    }
}
