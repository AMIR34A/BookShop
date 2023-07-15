using BookShop.Mapping;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Models
{
    public class BookShopContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Author_Book> Author_Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Order> Orders { get; set; }
        public OrderStatus OrderStatuses { get; set; }
        public DbSet<Order_Book> Order_Books { get; set; }
        public DbSet<Translator> Translators { get; set; }
        public DbSet<Book_Translator> Book_Translators { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Book_Category> Book_Categories { get; set; }
        //public DbQuery<ReadAllBook> ReadAllBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(false).UseSqlServer("Server=(local);Database=BookShopDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=true");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new Author_BookMap());
            modelBuilder.ApplyConfiguration(new Order_BookMap());
            modelBuilder.ApplyConfiguration(new Book_TranslatorMap());
            modelBuilder.ApplyConfiguration(new Book_CategoryMap());
            modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<Book>().Property(book => book.IsDeleted).HasDefaultValueSql("0");
            modelBuilder.Entity<Book>().Property(book => book.PublishedTime).HasDefaultValueSql("CONVERT(datetime,GetDate())");
            base.OnModelCreating(modelBuilder);
        }

        [DbFunction("GetAllAuthors", "dbo")]
        public static string GetAllAuthors(int bookId)
        {
            throw new NotImplementedException();
        }

        [DbFunction("GetAllTranslators", "dbo")]
        public static string GetAllTranslators(int bookId)
        {
            throw new NotImplementedException();
        }

        [DbFunction("GetAllCategories", "dbo")]
        public static string GetAllCategories(int bookId)
        {
            throw new NotImplementedException();
        }
    }
}
