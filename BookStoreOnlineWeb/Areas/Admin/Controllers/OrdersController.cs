using BookStoreOnline.Data.Repositories;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Models.ViewModels;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStoreOnlineWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrdersController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		[BindProperty]
		public OrderViewModel OrderViewModel { get; set; }

		public OrdersController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderViewModel = new OrderViewModel();

			OrderViewModel.OrderHeader = unitOfWork.OrderHeaderRepository
				.Get(x => x.Id == orderId, includeProperties: nameof(ApplicationUser));

			OrderViewModel.OrderDetails = unitOfWork.OrderDetailRepository
				.GetAll(x => x.OrderHeaderId == orderId, includeProperties: nameof(BookStoreOnline.Models.Product));

			return View(OrderViewModel);
		}

		[HttpPost]
		[Authorize(Roles = GlobalConstants.RoleAdmin + "," + GlobalConstants.RoleEmployee)]
		public IActionResult UpdateOrderDetails()
		{
			var orderHeader = unitOfWork.OrderHeaderRepository
				.Get(x => x.Id == OrderViewModel.OrderHeader.Id);

			orderHeader.Name = OrderViewModel.OrderHeader.Name;
			orderHeader.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
			orderHeader.Address = OrderViewModel.OrderHeader.Address;
			orderHeader.City = OrderViewModel.OrderHeader.City;
			orderHeader.Country = OrderViewModel.OrderHeader.Country;
			orderHeader.PostalCode = OrderViewModel.OrderHeader.PostalCode;

			if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier))
			{
				orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber))
			{
				orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
			}

			unitOfWork.OrderHeaderRepository.Update(orderHeader);
			unitOfWork.Save();

			TempData["success"] = "Order Details Updated Successfully.";

			return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = GlobalConstants.RoleAdmin + "," + GlobalConstants.RoleEmployee)]
		public IActionResult StartProcessing()
		{
			unitOfWork.OrderHeaderRepository
				.UpdateStatus(OrderViewModel.OrderHeader.Id, GlobalConstants.StatusInProcess);
			unitOfWork.Save();

			TempData["success"] = "Order Details Updated Successfully.";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = GlobalConstants.RoleAdmin + "," + GlobalConstants.RoleEmployee)]
		public IActionResult ShipOrder()
		{
			var orderHeader = unitOfWork.OrderHeaderRepository
				.Get(x => x.Id == OrderViewModel.OrderHeader.Id);

			orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
			orderHeader.OrderStatus = GlobalConstants.StatusShipped;
			orderHeader.ShippingDate = DateTime.UtcNow;

			if (orderHeader.OrderStatus == GlobalConstants.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateTime.UtcNow.AddDays(30);
			}

			unitOfWork.OrderHeaderRepository.Update(orderHeader);
			unitOfWork.Save();

			TempData["success"] = "Order Shipped Successfully.";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = GlobalConstants.RoleAdmin + "," + GlobalConstants.RoleEmployee)]
		public IActionResult CancelOrder()
		{
			var orderHeader = unitOfWork.OrderHeaderRepository
				.Get(x => x.Id == OrderViewModel.OrderHeader.Id);

			if (orderHeader.PaymentStatus == GlobalConstants.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions();
				options.Reason = RefundReasons.RequestedByCustomer;
				options.PaymentIntent = orderHeader.PaymentIntentId;

				var service = new RefundService();
				var refund = service.Create(options);

				unitOfWork.OrderHeaderRepository
					.UpdateStatus(orderHeader.Id, GlobalConstants.StatusCancelled, GlobalConstants.StatusRefunded);
			}
			else
			{
				unitOfWork.OrderHeaderRepository
					.UpdateStatus(orderHeader.Id, GlobalConstants.StatusCancelled, GlobalConstants.StatusCancelled);
			}

			unitOfWork.Save();
			TempData["success"] = "Order Cancelled Successfully.";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost, ActionName("Details")]
		public IActionResult DetailsPayNow()
		{
			OrderViewModel.OrderHeader = unitOfWork.OrderHeaderRepository
				.Get(x => x.Id == OrderViewModel.OrderHeader.Id, includeProperties: nameof(ApplicationUser));

			OrderViewModel.OrderDetails = unitOfWork.OrderDetailRepository
				.GetAll(x => x.OrderHeaderId == OrderViewModel.OrderHeader.Id, includeProperties: nameof(BookStoreOnline.Models.Product));

			var domain = "https://localhost:44372/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"Admin/Orders/PaymentConfirmation?orderHeaderId={OrderViewModel.OrderHeader.Id}",
				CancelUrl = domain + $"Admin/Orders/Details?orderId={OrderViewModel.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in OrderViewModel.OrderDetails)
			{
				var sessionLineItem = new SessionLineItemOptions();

				sessionLineItem.PriceData = new SessionLineItemPriceDataOptions();
				sessionLineItem.PriceData.UnitAmount = (long)(item.Price * 100);
				sessionLineItem.PriceData.Currency = "usd";
				sessionLineItem.PriceData.ProductData = new SessionLineItemPriceDataProductDataOptions();
				sessionLineItem.PriceData.ProductData.Name = item.Product.Title;
				sessionLineItem.Quantity = item.Count;

				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			var session = service.Create(options);

			unitOfWork.OrderHeaderRepository
				.UpdateStripePaymentId(OrderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);

			return new StatusCodeResult(303);
		}

		public IActionResult PaymentConfirmation(int orderHeaderId)
		{
			var orderHeader = unitOfWork.OrderHeaderRepository.Get(x => x.Id == orderHeaderId);

			if (orderHeader.PaymentStatus == GlobalConstants.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				var session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					unitOfWork.OrderHeaderRepository
						.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
					unitOfWork.OrderHeaderRepository
						.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, GlobalConstants.PaymentStatusApproved);
					unitOfWork.Save();
				}
			}

			return View(orderHeaderId);
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;

			if (User.IsInRole(GlobalConstants.RoleAdmin) || User.IsInRole(GlobalConstants.RoleEmployee))
			{
				orderHeaders = unitOfWork.OrderHeaderRepository
					.GetAll(includeProperties: nameof(ApplicationUser)).ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				orderHeaders = unitOfWork.OrderHeaderRepository
					.GetAll(x => x.ApplicationUserId == userId, includeProperties: nameof(ApplicationUser)).ToList();
			}

			switch (status)
			{
				case "pending":
					orderHeaders = orderHeaders.Where(x => x.PaymentStatus == GlobalConstants.PaymentStatusDelayedPayment);
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == GlobalConstants.StatusInProcess);
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == GlobalConstants.StatusShipped);
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == GlobalConstants.StatusApproved);
					break;
				default:
					break;
			}

			return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
