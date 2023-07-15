using EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Mapping
{
    public class Book_TranslatorMap : IEntityTypeConfiguration<Book_Translator>
    {
        public void Configure(EntityTypeBuilder<Book_Translator> builder)
        {
            builder.HasKey(p => new { p.BookId, p.TranslatorId });

            builder.HasOne(p => p.Book)
                .WithMany(p => p.Book_Translators)
                .HasForeignKey(p => p.BookId);

            builder.HasOne(p => p.Translator)
                .WithMany(p => p.Book_Translators)
                .HasForeignKey(p => p.TranslatorId);
        }
    }
}
