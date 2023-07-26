using BookShop.Models.Repository;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShop.Areas.Admin.Pages.Publishers;

public class EditModel : PageModel
{
    IUnitOfWork unitOfWork;

    public EditModel(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    [BindProperty]
    public Publisher Publisher { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Publisher = await unitOfWork.RepositoryBase<Publisher>().FindByIdAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        unitOfWork.RepositoryBase<Publisher>().Update(Publisher);
        await unitOfWork.SaveAsync();
        return RedirectToPage("./Index");
    }
}
