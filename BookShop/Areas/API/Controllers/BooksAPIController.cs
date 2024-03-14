using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksAPIController : ControllerBase
{
    IUnitOfWork _unitOfWork;

    public BooksAPIController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public List<BooksIndexViewModel> GetAllBooks() => _unitOfWork.BooksRepository.GetAllBooks("", "", "", "", "", "", "");

    [HttpPost]
    public async Task<string> CreateBook(BooksCreateEditViewModel viewModel) => await _unitOfWork.BooksRepository.CreateBookAsync(viewModel) ? "عملیات با موفقیت انجام شد." : "خطایی رخ داد.";

    [HttpPut]
    public async Task<string> EditBook(BooksCreateEditViewModel viewModel) => await _unitOfWork.BooksRepository.EditBookAsync(viewModel) ? "عملیات با موفقیت انجام شد." : "خطایی رخ داد.";

    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
        if (book is not null)
        {
            book.IsDeleted = true;
            return Content("عملیات با موفقیت انجام شد.");
        }
        return NotFound();
    }
}
