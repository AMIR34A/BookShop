using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Mapping
{
    public class Book_CategoryMap : IEntityTypeConfiguration<Book_Category>
    {
        public void Configure(EntityTypeBuilder<Book_Category> builder)
        {
            builder.HasKey(p => new { p.BookId, p.CategoryId });

            builder.HasOne(p => p.Book)
                .WithMany(p => p.Book_Categories)
                .HasForeignKey(p => p.BookId);

            builder.HasOne(p => p.Category)
                .WithMany(p => p.Book_Categories)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
