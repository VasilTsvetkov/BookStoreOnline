﻿// <auto-generated />
using BookStoreOnline.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookStoreOnline.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231107154944_ChangedNameOfPrice51To100PropertyInProductTable")]
    partial class ChangedNameOfPrice51To100PropertyInProductTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookStoreOnline.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "SciFi"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "History"
                        });
                });

            modelBuilder.Entity("BookStoreOnline.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ListPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Price51To100")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceOver100")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "F. Scott Fitzgerald",
                            CategoryId = 1,
                            Description = "A classic novel about the American Dream in the Jazz Age.",
                            ISBN = "978-0743273565",
                            ImageUrl = "",
                            ListPrice = 19.99m,
                            Price = 15.99m,
                            Price51To100 = 14.99m,
                            PriceOver100 = 13.99m,
                            Title = "The Great Gatsby"
                        },
                        new
                        {
                            Id = 2,
                            Author = "Harper Lee",
                            CategoryId = 2,
                            Description = "A powerful story about racial injustice in the American South.",
                            ISBN = "978-0061120084",
                            ImageUrl = "",
                            ListPrice = 18.99m,
                            Price = 16.99m,
                            Price51To100 = 15.99m,
                            PriceOver100 = 14.99m,
                            Title = "To Kill a Mockingbird"
                        },
                        new
                        {
                            Id = 3,
                            Author = "George Orwell",
                            CategoryId = 3,
                            Description = "A dystopian novel that explores the dangers of totalitarianism.",
                            ISBN = "978-0451524935",
                            ImageUrl = "",
                            ListPrice = 16.99m,
                            Price = 13.99m,
                            Price51To100 = 12.99m,
                            PriceOver100 = 11.99m,
                            Title = "1984"
                        },
                        new
                        {
                            Id = 4,
                            Author = "J.D. Salinger",
                            CategoryId = 2,
                            Description = "A coming-of-age novel following the adventures of Holden Caulfield.",
                            ISBN = "978-0316769174",
                            ImageUrl = "",
                            ListPrice = 15.99m,
                            Price = 12.99m,
                            Price51To100 = 11.99m,
                            PriceOver100 = 10.99m,
                            Title = "The Catcher in the Rye"
                        },
                        new
                        {
                            Id = 5,
                            Author = "Jane Austen",
                            CategoryId = 1,
                            Description = "A classic romance novel set in 19th-century England.",
                            ISBN = "978-0141439518",
                            ImageUrl = "",
                            ListPrice = 17.99m,
                            Price = 14.99m,
                            Price51To100 = 13.99m,
                            PriceOver100 = 12.99m,
                            Title = "Pride and Prejudice"
                        },
                        new
                        {
                            Id = 6,
                            Author = "J.R.R. Tolkien",
                            CategoryId = 2,
                            Description = "A fantasy adventure about the journey of Bilbo Baggins.",
                            ISBN = "978-0345534835",
                            ImageUrl = "",
                            ListPrice = 20.99m,
                            Price = 17.99m,
                            Price51To100 = 16.99m,
                            PriceOver100 = 15.99m,
                            Title = "The Hobbit"
                        },
                        new
                        {
                            Id = 7,
                            Author = "Paulo Coelho",
                            CategoryId = 1,
                            Description = "A philosophical novel about pursuing one's dreams.",
                            ISBN = "978-0062315007",
                            ImageUrl = "",
                            ListPrice = 14.99m,
                            Price = 11.99m,
                            Price51To100 = 10.99m,
                            PriceOver100 = 9.99m,
                            Title = "The Alchemist"
                        },
                        new
                        {
                            Id = 8,
                            Author = "J.K. Rowling",
                            CategoryId = 3,
                            Description = "The first book in the popular Harry Potter series.",
                            ISBN = "978-0590353427",
                            ImageUrl = "",
                            ListPrice = 21.99m,
                            Price = 18.99m,
                            Price51To100 = 17.99m,
                            PriceOver100 = 16.99m,
                            Title = "Harry Potter and the Sorcerer's Stone"
                        });
                });

            modelBuilder.Entity("BookStoreOnline.Models.Product", b =>
                {
                    b.HasOne("BookStoreOnline.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
