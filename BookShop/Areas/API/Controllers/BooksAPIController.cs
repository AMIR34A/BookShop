using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
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
    public List<BooksIndexViewModel> GetAllBooks() => _unitOfWork.BooksRepository.GetAllBooks("", "", "", "", "", "", "");
}
