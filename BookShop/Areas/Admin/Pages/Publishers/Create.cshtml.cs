using BookShop.Models.Repository;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShop.Areas.Admin.Pages.Publishers;

public class CreateModel : PageModel
{
    IUnitOfWork unitOfWork;

    public CreateModel(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    [BindProperty()]
    public Publisher Publisher { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await unitOfWork.RepositoryBase<Publisher>().CreateAsync(Publisher);
        await unitOfWork.SaveAsync();
        return RedirectToPage("./Index");
    }
}
