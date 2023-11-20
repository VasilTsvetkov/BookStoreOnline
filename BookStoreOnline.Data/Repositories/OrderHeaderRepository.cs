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
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private ApplicationDbContext db;

        public OrderHeaderRepository(ApplicationDbContext db)
			: base(db)
        {
            this.db = db;
        }

		public void Update(OrderHeader orderHeader)
		{
			db.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var order = db.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (order != null)
			{
				order.OrderStatus = orderStatus;

				if (!string.IsNullOrEmpty(paymentStatus))
				{
					order.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var order = db.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (!string.IsNullOrEmpty(sessionId))
			{
				order.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				order.PaymentIntentId = paymentIntentId;
				order.PaymentDate = DateTime.UtcNow;
			}
		}
	}
}
