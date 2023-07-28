using BookShop.Models.Repository;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShop.Areas.Admin.Pages.Publishers;

[Area("Admin")]
public class IndexModel : PageModel
{
    IUnitOfWork unitOfWork;
    public IndexModel(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IEnumerable<Publisher> Publishers { get; set; }
    public int PageSize { get; set; } = 2;
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int Count { get; set; }
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    public bool IsExistPreviousPage { get => CurrentPage > 1; }
    public bool IsExistNextPage { get => CurrentPage < TotalPages; }

    public async Task<IActionResult> OnGetAsync()
    {
        Count = await unitOfWork.RepositoryBase<Publisher>().CountAsync();
        Publishers = await unitOfWork.RepositoryBase<Publisher>().GetPaginateResaultAsync(CurrentPage, PageSize);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
        var publisher = await unitOfWork.RepositoryBase<Publisher>().FindByIdAsync(id);
        unitOfWork.RepositoryBase<Publisher>().Delete(publisher);
        await unitOfWork.SaveAsync();
        return RedirectToPage("/Publishers/Index");
    }
}
