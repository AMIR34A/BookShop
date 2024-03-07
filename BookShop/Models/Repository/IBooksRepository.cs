using BookShop.Models.ViewModels;

namespace BookShop.Models.Repository
{
    public interface IBooksRepository
    {
        void BindSubCategories(TreeViewCategory category);
        List<TreeViewCategory> GetAllCategories();
        List<BooksIndexViewModel> GetAllBooks(string title, string ISBN, string language, string author, string translator, string category, string publisher);
        Task<bool> CreateBookAsync(BooksCreateEditViewModel viewModel);
        Task<bool> EditBookAsync(BooksCreateEditViewModel viewModel);
    }
}
