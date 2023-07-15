using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Mapping
{
    public class Order_BookMap : IEntityTypeConfiguration<Order_Book>
    {
        public void Configure(EntityTypeBuilder<Order_Book> builder)
        {
            builder.HasKey(p => new { p.BookId, p.OrderId });

            builder.HasOne(p => p.Book)
                .WithMany(p => p.Order_Books)
                .HasForeignKey(p => p.BookId);

            builder.HasOne(p => p.Order)
                .WithMany(p => p.Order_Books)
                .HasForeignKey(p => p.OrderId);
        }
    }
}
