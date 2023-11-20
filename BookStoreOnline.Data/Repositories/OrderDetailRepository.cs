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
	public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
	{
		private ApplicationDbContext db;

        public OrderDetailRepository(ApplicationDbContext db)
			: base(db)
        {
            this.db = db;
        }

		public void Update(OrderDetail orderDetail)
		{
			db.OrderDetails.Update(orderDetail);
		}
	}
}
