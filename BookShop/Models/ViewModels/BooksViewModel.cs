using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models.ViewModels
{
    public class BookSubCategoriesViewModel
    {
        public BookSubCategoriesViewModel(List<TreeViewCategory> treeViewCategories, int[] categories)
        {
            Categories = categories;
            TreeViewCategories = treeViewCategories;
        }
        public int[] Categories { get; set; }
        public List<TreeViewCategory> TreeViewCategories { get; set; }
    }
    public class BooksCreateEditViewModel
    {
        public BooksCreateEditViewModel(IEnumerable<TreeViewCategory> viewCategories)
        {
            Categories = viewCategories;
        }

        public BooksCreateEditViewModel()
        {

        }

        public int BookId { get; set; }
        public BookSubCategoriesViewModel? BookSubCategoriesViewModel { get; set; }
        public IEnumerable<TreeViewCategory>? Categories { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "عنوان ")]
        public string Title { get; set; }

        [Display(Name = "خلاصه")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "قیمت")]
        public int Price { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "موجودی")]
        public int Stock { get; set; }
        public string Base64Image { get; set; }
        public IFormFile? Image { get; set; }

        [Display(Name = "تعداد صفحات")]
        public int NumOfPages { get; set; }

        [Display(Name = "وزن")]
        public short Weight { get; set; }

        [Display(Name = "شابک")]
        public string ISBN { get; set; }

        [Display(Name = " این کتاب روی سایت منتشر شود.")]
        public bool IsPublish { get; set; }


        [Display(Name = "سال انتشار")]
        public int PublishYear { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "زبان")]
        public int LanguageID { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "ناشر")]
        public int PublisherID { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "نویسندگان")]
        public int[] AuthorID { get; set; }

        [Display(Name = "مترجمان")]
        public int[] TranslatorID { get; set; }

        public int[] CategoryID { get; set; }
        public bool RecentIsPublish { get; set; }
        public DateTime? PublishDate { get; set; }

        [Required(ErrorMessage = "آپلود فایل کتاب الزامی است.")]
        public IFormFile File { get; set; }
        public string FileName { get; set; }
    }

    public class BooksIndexViewModel
    {
        public int BookId { get; set; }
        [Display(Name = "عنوان")]
        public string Title { get; set; }
        [Display(Name = "قیمت")]
        public int Price { get; set; }
        [Display(Name = "تعداد")]
        public int Stock { get; set; }
        [Display(Name = "شابک")]
        public string ISBN { get; set; }
        [Display(Name = "وضعیت")]
        public bool? IsPublish { get; set; }
        [Display(Name = "تاریخ انتشار")]
        public DateTime? PublishDate { get; set; }
        [Display(Name = "نویسندگان")]
        public string Authors { get; set; }
        [Display(Name = "ناشران")]
        public string PublisherName { get; set; }
        [Display(Name = "مترجمان")]
        public string Translators { get; set; }
        [Display(Name = "دسته بندی ها")]
        public string Categories { get; set; }
        public string Language { get; set; }
    }
    public class AuthorList
    {
        public int AuthorID { get; set; }
        public string NameFamily { get; set; }
    }

    public class TranslatorList
    {
        public int TranslatorID { get; set; }
        public string NameFamily { get; set; }
    }

    public class BooksAdvancedSearch
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Translator { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
    }

    [Keyless]
    public class ReadAllBook
    {
        public int BookId { get; set; }
        public string ISBN { get; set; }
        [Column(TypeName = "image")]
        public byte[]? Image { get; set; }
        public bool? IsPublished { get; set; }
        public int NumOfPage { get; set; }
        public int Price { get; set; }
        public DateTime? PublishedTime { get; set; }
        public int PublishYear { get; set; }
        public int Stock { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public short Weight { get; set; }
        public string LanguageName { get; set; }
        public string PublisherName { get; set; }
        public string? Authors { get; set; }
        public string? Translators { get; set; }
        public string? Categories { get; set; }
    }
}