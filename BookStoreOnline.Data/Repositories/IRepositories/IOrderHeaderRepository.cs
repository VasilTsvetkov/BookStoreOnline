using BookStoreOnline.Data.Repositories.IRepository;
using BookStoreOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreOnline.Data.Repositories.IRepositories
{
	public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);

		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
	}
}
