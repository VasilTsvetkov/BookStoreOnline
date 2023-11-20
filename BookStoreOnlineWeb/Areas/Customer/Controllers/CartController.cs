using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Models.ViewModels;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookStoreOnlineWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		[BindProperty]
		public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

		public CartController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel = new ShoppingCartViewModel();
			ShoppingCartViewModel.ShoppingCarts = unitOfWork.ShoppingCartRepository
				.GetAll(x => x.ApplicationUserId == userId, includeProperties: nameof(Product));
			ShoppingCartViewModel.OrderHeader = new OrderHeader();

			foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
			{
				cart.Price = CalculatePriceBasedOnQuantity(cart);
				ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

			return View(ShoppingCartViewModel);
		}

		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel = new ShoppingCartViewModel();
			ShoppingCartViewModel.ShoppingCarts = unitOfWork.ShoppingCartRepository
				.GetAll(x => x.ApplicationUserId == userId, includeProperties: nameof(Product));
			ShoppingCartViewModel.OrderHeader = new OrderHeader();

			ShoppingCartViewModel.OrderHeader.ApplicationUser = unitOfWork.ApplicationUserRepository
				.Get(x => x.Id == userId);

			ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
			ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartViewModel.OrderHeader.Address = ShoppingCartViewModel.OrderHeader.ApplicationUser.Address;
			ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
			ShoppingCartViewModel.OrderHeader.Country = ShoppingCartViewModel.OrderHeader.ApplicationUser.Country;
			ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
			{
				cart.Price = CalculatePriceBasedOnQuantity(cart);
				ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

			return View(ShoppingCartViewModel);
		}

		[HttpPost, ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel.ShoppingCarts = unitOfWork.ShoppingCartRepository
				.GetAll(x => x.ApplicationUserId == userId, includeProperties: nameof(Product));

			ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.UtcNow;
			ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

			var applicationUser = unitOfWork.ApplicationUserRepository
				.Get(x => x.Id == userId);

			foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
			{
				cart.Price = CalculatePriceBasedOnQuantity(cart);
				ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				ShoppingCartViewModel.OrderHeader.PaymentStatus = GlobalConstants.PaymentStatusPending;
				ShoppingCartViewModel.OrderHeader.OrderStatus = GlobalConstants.StatusPending;
			}
			else
			{
				ShoppingCartViewModel.OrderHeader.PaymentStatus = GlobalConstants.PaymentStatusDelayedPayment;
				ShoppingCartViewModel.OrderHeader.OrderStatus = GlobalConstants.StatusApproved;
			}

			unitOfWork.OrderHeaderRepository.Add(ShoppingCartViewModel.OrderHeader);
			unitOfWork.Save();

			foreach (var item in ShoppingCartViewModel.ShoppingCarts)
			{
				var orderDetail = new OrderDetail();
				orderDetail.ProductId = item.ProductId;
				orderDetail.OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id;
				orderDetail.Price = item.Price;
				orderDetail.Count = item.Count;

				unitOfWork.OrderDetailRepository.Add(orderDetail);
				unitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				var domain = "https://localhost:44372/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
					CancelUrl = domain + $"Customer/Cart/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach (var item in ShoppingCartViewModel.ShoppingCarts)
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
					.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
				unitOfWork.Save();

				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
		}

		public IActionResult OrderConfirmation(int id)
		{
			var orderHeader = unitOfWork.OrderHeaderRepository.Get(x => x.Id == id, includeProperties: nameof(ApplicationUser));

			if (orderHeader.PaymentStatus != GlobalConstants.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				var session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					unitOfWork.OrderHeaderRepository
						.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					unitOfWork.OrderHeaderRepository
						.UpdateStatus(id, GlobalConstants.StatusApproved, GlobalConstants.PaymentStatusApproved);
					unitOfWork.Save();
				}
			}

			var shoppingCarts = unitOfWork.ShoppingCartRepository
				.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId)
				.ToList();

			unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
			unitOfWork.Save();

			return View(id);
		}

		public IActionResult Plus(int cardId)
		{
			var cart = unitOfWork.ShoppingCartRepository.Get(x => x.Id == cardId);
			cart.Count++;
			unitOfWork.ShoppingCartRepository.Update(cart);
			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cardId)
		{
			var cart = unitOfWork.ShoppingCartRepository.Get(x => x.Id == cardId);

			if (cart.Count <= 1)
			{
				unitOfWork.ShoppingCartRepository.Remove(cart);
			}
			else
			{
				cart.Count--;
				unitOfWork.ShoppingCartRepository.Update(cart);
			}

			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cardId)
		{
			var cart = unitOfWork.ShoppingCartRepository.Get(x => x.Id == cardId);

			unitOfWork.ShoppingCartRepository.Remove(cart);
			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		private decimal CalculatePriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else
			{
				if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
				{
					return shoppingCart.Product.Price51To100;
				}
				else
				{
					return shoppingCart.Product.PriceOver100;
				}
			}
		}
	}
}
