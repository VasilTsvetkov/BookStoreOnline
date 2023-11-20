using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreOnline.Utilities
{
	public static class GlobalConstants
	{
		public const string RoleCustomer = "Customer";

		public const string RoleAdmin = "Admin";

		public const string RoleCompany = "Company";

		public const string RoleEmployee = "Employee";

		public const string StatusPending = "Pending";

		public const string StatusApproved = "Approved";

		public const string StatusInProcess = "Processing";

		public const string StatusShipped = "Shipped";

		public const string StatusCancelled = "Cancelled";

		public const string StatusRefunded = "Refunded";

		public const string PaymentStatusPending = "Pending";

		public const string PaymentStatusApproved = "Approved";

		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";

		public const string PaymentStatusRejected = "Rejected";
	}
}
