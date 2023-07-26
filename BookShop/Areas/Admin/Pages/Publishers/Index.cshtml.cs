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

    public async Task<IActionResult> OnGetAsync()
    {
        Publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
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
