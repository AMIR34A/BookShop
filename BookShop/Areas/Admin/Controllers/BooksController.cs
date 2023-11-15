using BookShop.Areas.Admin.Data;
using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.ComponentModel;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin"),DisplayName("مدیریت کتاب‌ها")]
    public class BooksController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public BooksController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [DisplayName("مشاهده کتاب‌ها")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Index(string Message, int pageIndex, int row = 5, string sortExpression = "Title", string title = "")
        {
            if (!string.IsNullOrEmpty(Message))
                ViewBag.Message = "خطایی رخ داده است، لطفا مجدد تلاش کنید";
            //var books = (from b in _context.Books.AsEnumerable()
            //             join p in _context.Publishers on b.PublisherId equals p.PublisherId
            //             join u in _context.Author_Books on b.BookId equals u.BookId
            //             join a in _context.Authors on u.AuthorId equals a.AuthorId
            //             where b.IsDeleted == false
            //             select new BooksIndexViewModel
            //             {
            //                 BookId = b.BookId,
            //                 Title = b.Title,
            //                 ISBN = b.ISBN,
            //                 Price = b.Price,
            //                 Stock = b.Stock,
            //                 IsPublish = b.IsPublished,
            //                 PublishDate = b.PublishedTime,
            //                 PublisherName = p.PublisherName,
            //                 Authors = $"{u.Author.FirstName} {u.Author.LastName}"
            //             }).GroupBy(b => b.BookId).Select(g => new { BookId = g.Key, BookGroups = g }).ToList();

            List<int> rows = new List<int> { 1, 2, 5, 10, 20, 50, 100 };


            ViewBag.RowId = new SelectList(rows, row);
            ViewBag.NumOfRow = (pageIndex - 1) * row + 1;
            ViewBag.Search = title;

            title = string.IsNullOrEmpty(title) ? "" : title;

            var paging = PagingList.Create(unitOfWork.BooksRepository.GetAllBooks(title, "", "", "", "", "", ""), row, pageIndex, sortExpression, "Title");

            paging.RouteValue = new RouteValueDictionary()
            {
                {"row",row },
                {"title",title }
            };

            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authors = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translators = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.Language = new SelectList(languages, "LanguageName", "LanguageName");
            ViewBag.Publisher = new SelectList(publishers, "PublisherName", "PublisherName");
            ViewBag.Author = new SelectList(authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "NameFamily", "NameFamily");
            ViewBag.Translator = new SelectList(translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "NameFamily", "NameFamily");
            ViewBag.Categories = unitOfWork.BooksRepository.GetAllCategories();
            return View(paging);
        }

        public IActionResult AdvancedSearch(BooksAdvancedSearch booksAdvancedSearch)
        {
            booksAdvancedSearch.Title = booksAdvancedSearch.Title == null ? "" : booksAdvancedSearch.Title;
            booksAdvancedSearch.ISBN = booksAdvancedSearch.ISBN == null ? "" : booksAdvancedSearch.ISBN;
            booksAdvancedSearch.Language = booksAdvancedSearch.Language == null ? "" : booksAdvancedSearch.Language;
            booksAdvancedSearch.Author = booksAdvancedSearch.Author == null ? "" : booksAdvancedSearch.Author;
            booksAdvancedSearch.Translator = booksAdvancedSearch.Translator == null ? "" : booksAdvancedSearch.Translator;
            booksAdvancedSearch.Category = booksAdvancedSearch.Category == null ? "" : booksAdvancedSearch.Category;
            booksAdvancedSearch.Publisher = booksAdvancedSearch.Publisher == null ? "" : booksAdvancedSearch.Publisher;

            return View(unitOfWork.BooksRepository.GetAllBooks(booksAdvancedSearch.Title, booksAdvancedSearch.ISBN, booksAdvancedSearch.Language, booksAdvancedSearch.Author, booksAdvancedSearch.Translator, booksAdvancedSearch.Category, booksAdvancedSearch.Publisher));
        }

        public IActionResult Details(int id)
        {
            //var bookInfo = _context.ReadAllBooks.Where(b => b.BookId == id).First();
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
            book.IsDeleted = true;
            await unitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        [DisplayName("افزودن کتاب")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Create()
        {
            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authors = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translators = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");
            BooksCreateEditViewModel viewModel = new BooksCreateEditViewModel() { BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), null) };
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateEditViewModel viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(viewModel);
            //}
            List<Book_Translator> translators = new List<Book_Translator>();
            List<Book_Category> categories = new List<Book_Category>();

            if (viewModel.TranslatorID is not null)
                translators = viewModel.TranslatorID.Select(translator => new Book_Translator { TranslatorId = translator }).ToList();
            if (viewModel.CategoryID is not null)
                categories = viewModel.CategoryID.Select(category => new Book_Category { CategoryId = category }).ToList();
            try
            {
                var transaction = await unitOfWork.BookShopContext.Database.BeginTransactionAsync(); ;
                Book book = new Book
                {
                    Title = viewModel.Title,
                    ISBN = viewModel.ISBN,
                    Summary = viewModel.Summary,
                    NumOfPage = viewModel.NumOfPages,
                    Stock = viewModel.Stock,
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

                await unitOfWork.RepositoryBase<Book>().CreateAsync(book);
                await unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index", new { Message = "Failde" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();
            var book = await unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
            if (book is null)
                return NotFound();
            var viewModel = (from b in unitOfWork.BookShopContext.Books.Include(l => l.Language).Include(p => p.Publisher).AsEnumerable()
                             where b.BookId == id
                             select new BooksCreateEditViewModel
                             {
                                 BookId = id.Value,
                                 Title = b.Title,
                                 ISBN = b.ISBN,
                                 IsPublish = b.IsPublished.Value,
                                 NumOfPages = b.NumOfPage,
                                 Price = b.Price,
                                 Stock = b.Stock,
                                 Summary = b.Summary,
                                 Weight = b.Weight,
                                 PublishYear = b.PublishYear,
                                 LanguageID = b.LanguageId,
                                 PublisherID = b.PublisherId,
                                 RecentIsPublish = b.IsPublished.Value,
                                 PublishDate = b.PublishedTime
                             }).First();

            var categories = (from c in unitOfWork.BookShopContext.Book_Categories
                              where c.BookId == id
                              select c.CategoryId).AsEnumerable().ToArray();

            var translators = (from t in unitOfWork.BookShopContext.Book_Translators
                               where t.BookId == id
                               select t.TranslatorId).AsEnumerable().ToArray();

            var authors = (from a in unitOfWork.BookShopContext.Author_Books
                           where a.BookId == id
                           select a.AuthorId).AsEnumerable().ToArray();

            viewModel.CategoryID = categories;
            viewModel.TranslatorID = translators;
            viewModel.AuthorID = authors;
            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), categories);

            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authorsV = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translatorsV = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authorsV.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translatorsV.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(BooksCreateEditViewModel viewModel)
        {
            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authorsV = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translatorsV = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), viewModel.CategoryID);
            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authorsV.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translatorsV.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");

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

                var categories = (from c in unitOfWork.BookShopContext.Book_Categories
                                  where c.BookId == viewModel.BookId
                                  select c.CategoryId).AsEnumerable().ToArray();

                var translators = (from t in unitOfWork.BookShopContext.Book_Translators
                                   where t.BookId == viewModel.BookId
                                   select t.TranslatorId).AsEnumerable().ToArray();

                var authors = (from a in unitOfWork.BookShopContext.Author_Books
                               where a.BookId == viewModel.BookId
                               select a.AuthorId).AsEnumerable().ToArray();

                var deletedCategories = categories.Except(viewModel.CategoryID);
                var deletedTranslators = translators.Except(viewModel.TranslatorID);
                var deletedAuthors = authors.Except(viewModel.AuthorID);

                var addedCategories = viewModel.CategoryID.Except(categories);
                var addedTranslators = viewModel.TranslatorID.Except(translators);
                var addedAuthors = viewModel.AuthorID.Except(authors);

                if (deletedCategories.Count() > 0)
                    unitOfWork.RepositoryBase<Book_Category>().DeleteRange(deletedCategories.Select(c => new Book_Category { BookId = viewModel.BookId, CategoryId = c }));

                if (deletedTranslators.Count() > 0)
                    unitOfWork.RepositoryBase<Book_Translator>().DeleteRange(deletedTranslators.Select(t => new Book_Translator { BookId = viewModel.BookId, TranslatorId = t }));

                if (deletedAuthors.Count() > 0)
                    unitOfWork.RepositoryBase<Author_Book>().DeleteRange(deletedAuthors.Select(a => new Author_Book { BookId = viewModel.BookId, AuthorId = a }));

                if (addedCategories.Count() > 0)
                    await unitOfWork.RepositoryBase<Book_Category>().CreateRangeAsync(addedCategories.Select(c => new Book_Category { BookId = viewModel.BookId, CategoryId = c }));

                if (addedTranslators.Count() > 0)
                    await unitOfWork.RepositoryBase<Book_Translator>().CreateRangeAsync(addedTranslators.Select(t => new Book_Translator { BookId = viewModel.BookId, TranslatorId = t }));

                if (addedAuthors.Count() > 0)
                    await unitOfWork.RepositoryBase<Author_Book>().CreateRangeAsync(addedAuthors.Select(a => new Author_Book { BookId = viewModel.BookId, AuthorId = a }));

                unitOfWork.RepositoryBase<Book>().Update(book);
                await unitOfWork.SaveAsync();
                ViewBag.MessageSuccess = "تغییرات با موفقیت ذخیره شد";
                return View(viewModel);
            }
            catch
            {
                ViewBag.MessageFail = "خطایی رخ داد، مجددا تلاش کنید";
                return View(viewModel);
            }
        }

        public async Task<IActionResult> SearchByISBN(string ISBN)
        {
            if (!string.IsNullOrEmpty(ISBN))
            {
                var book = await (from b in unitOfWork.BookShopContext.Books
                                  where b.ISBN == ISBN
                                  select new BooksIndexViewModel
                                  {
                                      Title = b.Title,
                                      Authors = BookShopContext.GetAllAuthors(b.BookId),
                                      Translators = BookShopContext.GetAllTranslators(b.BookId),
                                      Categories = BookShopContext.GetAllCategories(b.BookId)
                                  }).FirstAsync();
                if (book is null)
                {
                    ViewBag.Message = "کتابی با این شابک پیدا نشد";
                    return View();
                }
                return View(book);
            }
            return View();
        }
    }
}
