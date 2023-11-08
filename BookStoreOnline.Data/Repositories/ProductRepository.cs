using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreOnline.Data.Repositories
{
	public class ProductRepository : GenericRepository<Product>, IProductRepository
	{
		private ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db)
			: base(db)
        {
            this.db = db;
        }

		public void Update(Product product)
		{
			var productFromDb = db.Products.FirstOrDefault(x => x.Id == product.Id);

			if (productFromDb != null)
			{
				productFromDb.Title = product.Title;
				productFromDb.ISBN = product.ISBN;
				productFromDb.Price = product.Price;
				productFromDb.Price51To100 = product.Price51To100;
				productFromDb.PriceOver100 = product.PriceOver100;
				productFromDb.Description = product.Description;
				productFromDb.CategoryId = product.CategoryId;
				productFromDb.Author = product.Author;

				if (product.ImageUrl != null)
				{
					productFromDb.ImageUrl = product.ImageUrl;
				}
			}
		}
	}
}
