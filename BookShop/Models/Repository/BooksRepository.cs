using BookShop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using static BookShop.Models.ViewModels.CategoryViewModel;

namespace BookShop.Models.Repository
{
    public class BooksRepository
    {
        private readonly BookShopContext _context;

        public BooksRepository(BookShopContext context)
        {
            _context = context;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var subCategories = (from c in _context.Categories
                                 where c.CategoryParentId == category.id
                                 select new TreeViewCategory { id = c.CategoryId, title = c.CategoryName }).ToList();

            foreach (var item in subCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }
        }


        public List<TreeViewCategory> GetAllCategories()
        {
            var categories = (from c in _context.Categories
                              where c.ParentCategory == null
                              select new TreeViewCategory { id = c.CategoryId, title = c.CategoryName }).ToList();

            foreach (var item in categories)
            {
                BindSubCategories(item);
            }
            return categories;
        }

        public List<BooksIndexViewModel> GetAllBooks(string title, string ISBN, string language, string author, string translator, string category, string publisher)
        {
            var books = (from u in _context.Author_Books.Include(p => p.Book).ThenInclude(b => b.Publisher).Include(a => a.Author).AsEnumerable()
                         join l in _context.Languages on u.Book.Language.LanguageId equals l.LanguageId
                         join s in _context.Book_Translators on u.Book.BookId equals s.BookId into bt
                         from bts in bt.DefaultIfEmpty()
                         //join t in _context.Translators on bts.TranslatorId equals t.TranslatorId into tr
                         //from trl in tr.DefaultIfEmpty()
                         //join r in _context.Book_Categories on u.Book.BookId equals r.BookId into bc
                         //from bct in bc.DefaultIfEmpty()
                         //join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                         //from cog in cg.DefaultIfEmpty()
                         where u.Book.Title.Contains(title.TrimStart().TrimEnd())
                         && u.Book.ISBN.Contains(ISBN.TrimStart().TrimEnd())
                         //&& EF.Functions.Like(l.LanguageName, "%" + language + "%")
                         && u.Book.Publisher.PublisherName.Contains(publisher.TrimStart().TrimEnd())
                         select new BooksIndexViewModel
                         {
                             BookId = u.Book.BookId,
                             Title = u.Book.Title,
                             ISBN = u.Book.ISBN,
                             Price = u.Book.Price,
                             Stock = u.Book.Stock,
                             IsPublish = u.Book.IsPublished,
                             PublishDate = u.Book.PublishedTime,
                             PublisherName = u.Book.Publisher.PublisherName,
                             Authors = $"{u.Author.FirstName} {u.Author.LastName}",
                             //Translator = bts != null ? trl.FirstName + " " + trl.LastName : "",
                             //Category = bct != null ? cog.CategoryName : "",
                         }).GroupBy(b => b.BookId).Select(g => new { BookId = g.Key, BookGroups = g }).ToList();

            List<BooksIndexViewModel> allBooks = new List<BooksIndexViewModel>();
            foreach (var book in books)
            {
                var authors = "";
                var translatorName = "";
                var categoryName = "";

                foreach (var bookGroup in book.BookGroups.Select(a=>a.Authors).Distinct())
                {
                    authors += authors == "" ? bookGroup : $"- {bookGroup}";
                }

                foreach (var bookGroup in book.BookGroups.Select(a => a.Translators).Distinct())
                {
                    translatorName += translatorName == "" ? bookGroup : $"- {bookGroup}";
                }

                foreach (var bookGroup in book.BookGroups.Select(a => a.Categories).Distinct())
                {
                    categoryName += categoryName == "" ? bookGroup : $"- {bookGroup}";
                }

                var bookInfo = new BooksIndexViewModel
                {
                    BookId = book.BookId,
                    Title = book.BookGroups.First().Title,
                    ISBN = book.BookGroups.First().ISBN,
                    Price = book.BookGroups.First().Price,
                    Stock = book.BookGroups.First().Stock,
                    IsPublish = book.BookGroups.First().IsPublish,
                    PublishDate = book.BookGroups.First().PublishDate,
                    PublisherName = book.BookGroups.First().PublisherName,
                    Authors = authors,
                    Translators = translatorName,
                    Categories = categoryName,
                    Language = book.BookGroups.First().Language

                    
                };
                allBooks.Add(bookInfo);
            }

            return allBooks;
        }
    }
}
