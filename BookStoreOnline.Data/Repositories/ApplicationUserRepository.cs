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
	public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
		private ApplicationDbContext db;

        public ApplicationUserRepository(ApplicationDbContext db)
			: base(db)
        {
            this.db = db;
        }

		public void Update(ApplicationUser applicationUser)
		{
			db.ApplicationUsers.Update(applicationUser);
		}
	}
}
