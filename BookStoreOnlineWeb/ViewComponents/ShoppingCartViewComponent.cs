using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreOnlineWeb.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		private readonly IUnitOfWork unitOfWork;

		public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				if (HttpContext.Session.GetInt32(GlobalConstants.SessionCart) != null)
				{
					HttpContext.Session.SetInt32(GlobalConstants.SessionCart,
						unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value).Count());
				}

				return View(HttpContext.Session.GetInt32(GlobalConstants.SessionCart));
			}
			else
			{
				HttpContext.Session.Clear();
				return View(0);
			}
		}
	}
}
