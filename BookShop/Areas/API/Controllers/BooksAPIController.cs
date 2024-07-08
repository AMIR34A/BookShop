using Asp.Versioning;
using BookShop.Areas.API.Classes;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class BooksAPIController : ControllerBase
{
    IUnitOfWork _unitOfWork;

    public BooksAPIController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public APIResult<List<BooksIndexViewModel>> GetAllBooks() => _unitOfWork.BooksRepository.GetAllBooks("", "", "", "", "", "", "");

    [HttpPost]
    public async Task<APIResult<string>> CreateBook(BooksCreateEditViewModel viewModel) => await _unitOfWork.BooksRepository.CreateBookAsync(viewModel) ? Ok("عملیات با موفقیت انجام شد.") : BadRequest("خطایی رخ داد.");

    [HttpPut]
    public async Task<APIResult<string>> EditBook(BooksCreateEditViewModel viewModel) => await _unitOfWork.BooksRepository.EditBookAsync(viewModel) ? Ok("عملیات با موفقیت انجام شد.") : BadRequest("خطایی رخ داد.");

    [HttpDelete]
    public async Task<APIResult<string>> DeleteBook(int id)
    {
        var book = await _unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
        if (book is not null)
        {
            book.IsDeleted = true;
            return Ok("عملیات با موفقیت انجام شد.");
        }
        return BadRequest();
    }
}
