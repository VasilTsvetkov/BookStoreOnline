using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStoreOnlineWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
			var products = unitOfWork.ProductRepository.GetAll(includeProperties: nameof(Category) + "," + nameof(Product.ProductImages));
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.Product = unitOfWork.ProductRepository
                .Get(x => x.Id == id, includeProperties: nameof(Category) + "," + nameof(Product.ProductImages));
            shoppingCart.Count = 1;
            shoppingCart.ProductId = id;

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            var dbCart = unitOfWork.ShoppingCartRepository
                .Get(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);

            if (dbCart != null)
            {
                dbCart.Count += shoppingCart.Count;
                unitOfWork.ShoppingCartRepository.Update(dbCart);
				unitOfWork.Save();
			}
            else
            {
                unitOfWork.ShoppingCartRepository.Add(shoppingCart);
				unitOfWork.Save();
				HttpContext.Session.SetInt32(GlobalConstants.SessionCart, 
                    unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == userId).Count());
            }

            TempData["success"] = "Cart updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}