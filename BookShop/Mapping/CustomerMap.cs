using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Mapping
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasOne(p => p.FirstCity)
                .WithMany(p => p.FirstCustomers)
                .HasForeignKey(p => p.FirstCityId);

            builder.HasOne(p => p.SecondCity)
                .WithMany(p => p.SecondCustomers)
                .HasForeignKey(p => p.SecondCityId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
