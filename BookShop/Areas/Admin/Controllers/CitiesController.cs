using BookShop.Models.Repository;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class CitiesController : Controller
{
    IUnitOfWork unitOfWork;

    public CitiesController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int? id)
    {
        var province = await unitOfWork.RepositoryBase<Province>().FindByIdAsync(id.Value);
        await unitOfWork.BookShopContext.Entry(province).Collection(p => p.Cities).LoadAsync();
        return View(province);
    }
}
