using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Areas.Admin.Controllers;

//[Area("Admin")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TranslatorsController : Controller
{
    IUnitOfWork unitOfWork;

    public TranslatorsController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        return View(await unitOfWork.RepositoryBase<Translator>().GetAllAsync());
    }

    //[HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TranslatorsCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        Translator translator = new Translator
        {
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName
        };
        await unitOfWork.RepositoryBase<Translator>().CreateAsync(translator);
        await unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var translator = await unitOfWork.RepositoryBase<Translator>().FindByIdAsync(id.Value);
        if (translator == null)
            return NotFound();

        return View(translator);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Translator translator)
    {
        //if (!ModelState.IsValid)
        //    return NotFound();
        unitOfWork.RepositoryBase<Translator>().Update(translator);
        await unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var translator = await unitOfWork.RepositoryBase<Translator>().FindByIdAsync(id.Value);
        return View(translator);
    }

    [HttpPost]
    public async Task<IActionResult> Deleted(int? id)
    {
        var translator = await unitOfWork.RepositoryBase<Translator>().FindByIdAsync(id.Value);
        unitOfWork.RepositoryBase<Translator>().Delete(translator);
        await unitOfWork.SaveAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue)
            return NotFound();

        var traslator = await unitOfWork.RepositoryBase<Translator>().FindByIdAsync(id.Value);
        return View(traslator);
    }
}
