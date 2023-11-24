using BookStoreOnline.Data.Repositories;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Models.ViewModels;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
				.GetAll(x => x.OrderHeaderId == orderId, includeProperties: nameof(Product));

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

			TempData["success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id});
		}

		[HttpPost]
		[Authorize(Roles = GlobalConstants.RoleAdmin + "," + GlobalConstants.RoleEmployee)]
		public IActionResult StartProcessing()
		{
			unitOfWork.OrderHeaderRepository
				.UpdateStatus(OrderViewModel.OrderHeader.Id, GlobalConstants.StatusInProcess);
			unitOfWork.Save();

			TempData["success"] = "Order Details Updated Successfully";

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

			unitOfWork.OrderHeaderRepository
				.Update(orderHeader);
			unitOfWork.Save();

			TempData["success"] = "Order Shipped Successfully";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
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
