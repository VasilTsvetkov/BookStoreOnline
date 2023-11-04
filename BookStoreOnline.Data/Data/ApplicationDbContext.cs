using BookStoreOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreOnline.Data.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}

		public DbSet<Category> Categories { get; set; }

		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Action", DisplayOrder = 1, },
				new Category { Id = 2, Name = "SciFi", DisplayOrder = 2, },
				new Category { Id = 3, Name = "History", DisplayOrder = 3, }
				);

			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					Id = 1,
					Title = "The Great Gatsby",
					Author = "F. Scott Fitzgerald",
					Description = "A classic novel about the American Dream in the Jazz Age.",
					ISBN = "978-0743273565",
					ListPrice = 19.99M,
					Price = 15.99M,
					Price50To100 = 14.99M,
					PriceOver100 = 13.99M,
					CategoryId = 1,
					ImageUrl = "",
				},
				new Product
				{
					Id = 2,
					Title = "To Kill a Mockingbird",
					Author = "Harper Lee",
					Description = "A powerful story about racial injustice in the American South.",
					ISBN = "978-0061120084",
					ListPrice = 18.99M,
					Price = 16.99M,
					Price50To100 = 15.99M,
					PriceOver100 = 14.99M,
					CategoryId = 2,
					ImageUrl = "",
				},
				new Product
				{
					Id = 3,
					Title = "1984",
					Author = "George Orwell",
					Description = "A dystopian novel that explores the dangers of totalitarianism.",
					ISBN = "978-0451524935",
					ListPrice = 16.99M,
					Price = 13.99M,
					Price50To100 = 12.99M,
					PriceOver100 = 11.99M,
					CategoryId = 3,
					ImageUrl = "",
				},
				new Product
				{
					Id = 4,
					Title = "The Catcher in the Rye",
					Author = "J.D. Salinger",
					Description = "A coming-of-age novel following the adventures of Holden Caulfield.",
					ISBN = "978-0316769174",
					ListPrice = 15.99M,
					Price = 12.99M,
					Price50To100 = 11.99M,
					PriceOver100 = 10.99M,
					CategoryId = 2,
					ImageUrl = "",
				},
				new Product
				{
					Id = 5,
					Title = "Pride and Prejudice",
					Author = "Jane Austen",
					Description = "A classic romance novel set in 19th-century England.",
					ISBN = "978-0141439518",
					ListPrice = 17.99M,
					Price = 14.99M,
					Price50To100 = 13.99M,
					PriceOver100 = 12.99M,
					CategoryId = 1,
					ImageUrl = "",
				},
				new Product
				{
					Id = 6,
					Title = "The Hobbit",
					Author = "J.R.R. Tolkien",
					Description = "A fantasy adventure about the journey of Bilbo Baggins.",
					ISBN = "978-0345534835",
					ListPrice = 20.99M,
					Price = 17.99M,
					Price50To100 = 16.99M,
					PriceOver100 = 15.99M,
					CategoryId = 2,
					ImageUrl = "",
				},
				new Product
				{
					Id = 7,
					Title = "The Alchemist",
					Author = "Paulo Coelho",
					Description = "A philosophical novel about pursuing one's dreams.",
					ISBN = "978-0062315007",
					ListPrice = 14.99M,
					Price = 11.99M,
					Price50To100 = 10.99M,
					PriceOver100 = 9.99M,
					CategoryId = 1,
					ImageUrl = "",
				},
				new Product
				{
					Id = 8,
					Title = "Harry Potter and the Sorcerer's Stone",
					Author = "J.K. Rowling",
					Description = "The first book in the popular Harry Potter series.",
					ISBN = "978-0590353427",
					ListPrice = 21.99M,
					Price = 18.99M,
					Price50To100 = 17.99M,
					PriceOver100 = 16.99M,
					CategoryId = 3,
					ImageUrl = "",
				}
			);
		}
	}
}
