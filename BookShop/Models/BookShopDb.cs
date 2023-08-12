using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCore.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string? File { get; set; }
        [Column(TypeName = "image")]
        public byte[]? Image { get; set; }
        public int NumOfPage { get; set; }
        public short Weight { get; set; }
        public string ISBN { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime? PublishedTime { get; set; }
        public int PublishYear { get; set; }
        public int PublisherId { get; set; }
        public bool IsDeleted { get; set; }

        //second method
        //[ForeignKey("Category")]
        //public int CategoryId { get; set; }
        //public Category Category { get; set; }

        //third method
        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public Discount Discount { get; set; }

        public List<Author_Book> Author_Books { get; set; }

        public List<Order_Book> Order_Books { get; set; }

        public List<Book_Translator> Book_Translators { get; set; }

        public List<Book_Category> Book_Categories { get; set; }

        public Publisher Publisher { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        [ForeignKey("ParentCategory")]
        public int? CategoryParentId { get; set; }

        public Category ParentCategory { get; set; }

        public List<Category> Categories { get; set; }
        public List<Book_Category> Book_Categories { get; set; }
    }

    public class Book_Category
    {
        public int BookId { get; set; }
        public int CategoryId { get; set; }

        public Book Book { get; set; }
        public Category Category { get; set; }

    }
    public class Language
    {
        [Key]
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public List<Book> Books { get; set; }
    }

    public class Discount
    {
        [Key, ForeignKey("Book")]
        public int BookId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte Percent { get; set; }

        public Book Book { get; set; }
    }

    public class Author
    {
        private ILazyLoader _lazyLoader;
        private List<Author_Book> author_Books;
        public Author()
        {

        }

        private Author(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }
        [Key]
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Author_Book> Author_Books
        {
            get => _lazyLoader.Load(this, ref author_Books);
            set => author_Books = value;
        }
    }

    public class Author_Book
    {
        private ILazyLoader _lazyLoader;
        private Book book;
        public Author_Book()
        {

        }

        private Author_Book(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public Book Book
        {
            get => _lazyLoader.Load(this, ref book);
            set => book = value;
        }
        public Author Author { get; set; }
    }

    public class Customer
    {
        [Key]
        public string CustomerId { get; set; }
        public string FirstAddress { get; set; }
        public string SecondAddress { get; set; }
        public string FirstPostalCode { get; set; }
        public string SecondPostalCode { get; set; }
        public string Tellphone { get; set; }
        public string Image { get; set; }

        public int FirstCityId { get; set; }
        public int SecondCityId { get; set; }
        public City FirstCity { get; set; }
        public City SecondCity { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Province
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProvinceId { get; set; }
        [Display(Name = "استان")]
        public string ProvinceName { get; set; }

        public List<City> Cities { get; set; }
    }

    public class City
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CityId { get; set; }
        public string CityName { get; set; }

        public Province Province { get; set; }

        public List<Customer> FirstCustomers { get; set; }
        public List<Customer> SecondCustomers { get; set; }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public long AmountPaid { get; set; }
        public string DispatchNumber { get; set; }
        public DateTime BuyDate { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public Customer Customer { get; set; }

        public List<Order_Book> Order_Books { get; set; }
    }

    public class OrderStatus
    {
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }

        public List<Order> Orders { get; set; }
    }

    public class Order_Book
    {
        public int BookId { get; set; }
        public string OrderId { get; set; }

        public Book Book { get; set; }
        public Order Order { get; set; }
    }

    public class Publisher
    {
        public int PublisherId { get; set; }
        [Display(Name = "ناشر")]
        [Required(ErrorMessage = "فیلد {0} باید پر شود")]
        public string PublisherName { get; set; }

        public List<Book> Books { get; set; }
    }

    public class Translator
    {
        public int TranslatorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Book_Translator> Book_Translators { get; set; }
    }

    public class Book_Translator
    {
        public int BookId { get; set; }
        public int TranslatorId { get; set; }
        public Book Book { get; set; }
        public Translator Translator { get; set; }
    }

}
