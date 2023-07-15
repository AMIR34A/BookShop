using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Mapping
{
    public class Author_BookMap : IEntityTypeConfiguration<Author_Book>
    {
        public void Configure(EntityTypeBuilder<Author_Book> builder)
        {
            builder.HasKey(p => new { p.BookId, p.AuthorId });

            builder.HasOne(p => p.Author)
                .WithMany(p => p.Author_Books)
                .HasForeignKey(p => p.AuthorId);

            builder.HasOne(p => p.Book)
                .WithMany(p => p.Author_Books)
                .HasForeignKey(p => p.BookId);
        }
    }
}
