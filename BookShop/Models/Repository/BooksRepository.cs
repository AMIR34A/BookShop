using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Models.Repository;

public class BooksRepository : IBooksRepository
{
    private readonly BookShopContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public BooksRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _context = _unitOfWork.BookShopContext;
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

            foreach (var bookGroup in book.BookGroups.Select(a => a.Authors).Distinct())
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

    public async Task<bool> CreateBookAsync(BooksCreateEditViewModel viewModel)
    {
        List<Book_Translator> translators = new List<Book_Translator>();
        List<Book_Category> categories = new List<Book_Category>();

        if (viewModel.TranslatorID is not null)
            translators = viewModel.TranslatorID.Select(translator => new Book_Translator { TranslatorId = translator }).ToList();
        if (viewModel.CategoryID is not null)
            categories = viewModel.CategoryID.Select(category => new Book_Category { CategoryId = category }).ToList();
        try
        {
            var transaction = await _unitOfWork.BookShopContext.Database.BeginTransactionAsync();
            Book book = new Book
            {
                Title = viewModel.Title,
                ISBN = viewModel.ISBN,
                Summary = viewModel.Summary,
                NumOfPage = viewModel.NumOfPages,
                Stock = viewModel.Stock,
                Image = !string.IsNullOrEmpty(viewModel.Base64Image) ? Convert.FromBase64String(viewModel.Base64Image) : null,
                Price = viewModel.Price,
                LanguageId = viewModel.LanguageID,
                IsPublished = viewModel.IsPublish,
                Weight = viewModel.Weight,
                PublishYear = viewModel.PublishYear,
                PublisherId = viewModel.PublisherID,
                Author_Books = viewModel.AuthorID.Select(author => new Author_Book { AuthorId = author }).ToList(),
                Book_Translators = translators,
                Book_Categories = categories
            };

            await _unitOfWork.RepositoryBase<Book>().CreateAsync(book);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> EditBookAsync(BooksCreateEditViewModel viewModel)
    {
        DateTime? dateTime;
        if (!viewModel.RecentIsPublish && viewModel.IsPublish)
            dateTime = DateTime.Now;
        else if (viewModel.RecentIsPublish && !viewModel.IsPublish)
            dateTime = null;
        else
            dateTime = viewModel.PublishDate;
        try
        {
            var book = new Book
            {
                BookId = viewModel.BookId,
                Title = viewModel.Title,
                ISBN = viewModel.ISBN,
                IsPublished = viewModel.IsPublish,
                NumOfPage = viewModel.NumOfPages,
                Price = viewModel.Price,
                Stock = viewModel.Stock,
                Summary = viewModel.Summary,
                Weight = viewModel.Weight,
                PublishYear = viewModel.PublishYear,
                LanguageId = viewModel.LanguageID,
                PublisherId = viewModel.PublisherID,
                PublishedTime = dateTime
            };

            var categories = (from c in _unitOfWork.BookShopContext.Book_Categories
                              where c.BookId == viewModel.BookId
                              select c.CategoryId).AsEnumerable().ToArray();

            var translators = (from t in _unitOfWork.BookShopContext.Book_Translators
                               where t.BookId == viewModel.BookId
                               select t.TranslatorId).AsEnumerable().ToArray();

            var authors = (from a in _unitOfWork.BookShopContext.Author_Books
                           where a.BookId == viewModel.BookId
                           select a.AuthorId).AsEnumerable().ToArray();

            if (viewModel.CategoryID is null)
                viewModel.CategoryID = Array.Empty<int>();
            if (viewModel.TranslatorID is null)
                viewModel.TranslatorID = Array.Empty<int>();

            var deletedCategories = categories.Except(viewModel.CategoryID);
            var deletedTranslators = translators.Except(viewModel.TranslatorID);
            var deletedAuthors = authors.Except(viewModel.AuthorID);

            var addedCategories = viewModel.CategoryID.Except(categories);
            var addedTranslators = viewModel.TranslatorID.Except(translators);
            var addedAuthors = viewModel.AuthorID.Except(authors);

            if (deletedCategories.Count() > 0)
                _unitOfWork.RepositoryBase<Book_Category>().DeleteRange(deletedCategories.Select(c => new Book_Category { BookId = viewModel.BookId, CategoryId = c }));

            if (deletedTranslators.Count() > 0)
                _unitOfWork.RepositoryBase<Book_Translator>().DeleteRange(deletedTranslators.Select(t => new Book_Translator { BookId = viewModel.BookId, TranslatorId = t }));

            if (deletedAuthors.Count() > 0)
                _unitOfWork.RepositoryBase<Author_Book>().DeleteRange(deletedAuthors.Select(a => new Author_Book { BookId = viewModel.BookId, AuthorId = a }));

            if (addedCategories.Count() > 0)
                await _unitOfWork.RepositoryBase<Book_Category>().CreateRangeAsync(addedCategories.Select(c => new Book_Category { BookId = viewModel.BookId, CategoryId = c }));

            if (addedTranslators.Count() > 0)
                await _unitOfWork.RepositoryBase<Book_Translator>().CreateRangeAsync(addedTranslators.Select(t => new Book_Translator { BookId = viewModel.BookId, TranslatorId = t }));

            if (addedAuthors.Count() > 0)
                await _unitOfWork.RepositoryBase<Author_Book>().CreateRangeAsync(addedAuthors.Select(a => new Author_Book { BookId = viewModel.BookId, AuthorId = a }));

            _unitOfWork.RepositoryBase<Book>().Update(book);
            await _unitOfWork.SaveAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
