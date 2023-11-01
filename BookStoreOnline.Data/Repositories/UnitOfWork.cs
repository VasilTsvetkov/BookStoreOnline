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

		public UnitOfWork(ApplicationDbContext db)
		{
			this.db = db;
			CategoryRepository = new CategoryRepository(this.db);
		}

		public void Save()
		{
			db.SaveChanges();
		}
	}
}
