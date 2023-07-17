using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static BookShop.Models.ViewModels.CategoryViewModel;

namespace BookShop.Models.Repository
{
    public interface IBooksRepository
    {
        void BindSubCategories(TreeViewCategory category);
        List<TreeViewCategory> GetAllCategories();
        List<BooksIndexViewModel> GetAllBooks(string title, string ISBN, string language, string author, string translator, string category, string publisher);
    }
}
