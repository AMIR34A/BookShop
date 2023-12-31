﻿// <auto-generated />
using System;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookShop.Migrations
{
    [DbContext(typeof(BookShopContext))]
    [Migration("20220920060947_Edit_Publisher_Column")]
    partial class Edit_Publisher_Column
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EntityFrameworkCore.Models.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AuthorId"), 1L, 1);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Author_Book", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.HasKey("BookId", "AuthorId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Author_Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookId"), 1L, 1);

                    b.Property<string>("File")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("image");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsPublished")
                        .HasColumnType("bit");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<int>("NumOfPage")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("PublishYear")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PublishedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Weight")
                        .HasColumnType("smallint");

                    b.HasKey("BookId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("PublisherId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book_Category", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("BookId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Book_Categories");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book_Translator", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<int>("TranslatorId")
                        .HasColumnType("int");

                    b.HasKey("BookId", "TranslatorId");

                    b.HasIndex("TranslatorId");

                    b.ToTable("Book_Translators");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CategoryParentId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.HasIndex("CategoryParentId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.City", b =>
                {
                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.HasKey("CityId");

                    b.HasIndex("ProvinceId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Customer", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FirstCityId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecondCityId")
                        .HasColumnType("int");

                    b.Property<string>("Tellphone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomerId");

                    b.HasIndex("FirstCityId");

                    b.HasIndex("SecondCityId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Discount", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<byte>("Percent")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("BookId");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Language", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LanguageId"), 1L, 1);

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LanguageId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Order", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("AmountPaid")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("BuyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DispatchNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderStatusId")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("OrderStatusId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Order_Book", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BookId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("Order_Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.OrderStatus", b =>
                {
                    b.Property<int>("OrderStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderStatusId"), 1L, 1);

                    b.Property<string>("OrderStatusName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderStatusId");

                    b.ToTable("OrderStatus");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Province", b =>
                {
                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("ProvinceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProvinceId");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Publisher", b =>
                {
                    b.Property<int>("PublisherId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PublisherId"), 1L, 1);

                    b.Property<string>("PublisherName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PublisherId");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Translator", b =>
                {
                    b.Property<int>("TranslatorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TranslatorId"), 1L, 1);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TranslatorId");

                    b.ToTable("Translators");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Author_Book", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Author", "Author")
                        .WithMany("Author_Books")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.Book", "Book")
                        .WithMany("Author_Books")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Language", "Language")
                        .WithMany("Books")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book_Category", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Book", "Book")
                        .WithMany("Book_Categories")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.Category", "Category")
                        .WithMany("Book_Categories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book_Translator", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Book", "Book")
                        .WithMany("Book_Translators")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.Translator", "Translator")
                        .WithMany("Book_Translators")
                        .HasForeignKey("TranslatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Translator");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Category", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Category", "ParentCategory")
                        .WithMany("Categories")
                        .HasForeignKey("CategoryParentId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.City", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Province", "Province")
                        .WithMany("Cities")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Province");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Customer", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.City", "FirstCity")
                        .WithMany("FirstCustomers")
                        .HasForeignKey("FirstCityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.City", "SecondCity")
                        .WithMany("SecondCustomers")
                        .HasForeignKey("SecondCityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FirstCity");

                    b.Navigation("SecondCity");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Discount", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Book", "Book")
                        .WithOne("Discount")
                        .HasForeignKey("EntityFrameworkCore.Models.Discount", "BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Order", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.OrderStatus", "OrderStatus")
                        .WithMany("Orders")
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("OrderStatus");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Order_Book", b =>
                {
                    b.HasOne("EntityFrameworkCore.Models.Book", "Book")
                        .WithMany("Order_Books")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkCore.Models.Order", "Order")
                        .WithMany("Order_Books")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Author", b =>
                {
                    b.Navigation("Author_Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Book", b =>
                {
                    b.Navigation("Author_Books");

                    b.Navigation("Book_Categories");

                    b.Navigation("Book_Translators");

                    b.Navigation("Discount")
                        .IsRequired();

                    b.Navigation("Order_Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Category", b =>
                {
                    b.Navigation("Book_Categories");

                    b.Navigation("Categories");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.City", b =>
                {
                    b.Navigation("FirstCustomers");

                    b.Navigation("SecondCustomers");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Language", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Order", b =>
                {
                    b.Navigation("Order_Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.OrderStatus", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Province", b =>
                {
                    b.Navigation("Cities");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Publisher", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("EntityFrameworkCore.Models.Translator", b =>
                {
                    b.Navigation("Book_Translators");
                });
#pragma warning restore 612, 618
        }
    }
}
