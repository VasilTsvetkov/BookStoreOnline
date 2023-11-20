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
	public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
		private ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext db)
			: base(db)
        {
            this.db = db;
        }

		public void Update(Company company)
		{
			db.Companies.Update(company);
		}
	}
}
