using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreOnline.Data.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private ApplicationDbContext db;

		public ICategoryRepository CategoryRepository { get; private set; }

		public IProductRepository ProductRepository { get; private set; }

		public ICompanyRepository CompanyRepository { get; private set; }

		public IShoppingCartRepository ShoppingCartRepository { get; private set; }

		public IApplicationUserRepository ApplicationUserRepository { get; private set; }

		public IOrderDetailRepository OrderDetailRepository { get; private set; }

		public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

		public IProductImageRepository ProductImageRepository { get; private set; }

		public UnitOfWork(ApplicationDbContext db)
		{
			this.db = db;
			CategoryRepository = new CategoryRepository(this.db);
			ProductRepository = new ProductRepository(this.db);
            CompanyRepository = new CompanyRepository(this.db);
			ShoppingCartRepository = new ShoppingCartRepository(this.db);
			ApplicationUserRepository = new ApplicationUserRepository(this.db);
			OrderDetailRepository = new OrderDetailRepository(this.db);
			OrderHeaderRepository = new OrderHeaderRepository(this.db);
			ProductImageRepository = new ProductImageRepository(this.db);
		}

		public void Save()
		{
			db.SaveChanges();
		}
	}
}
