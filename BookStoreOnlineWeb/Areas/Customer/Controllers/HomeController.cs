using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
			var products = unitOfWork.ProductRepository.GetAll(includeProperties: nameof(Category));
			return View(products);
		}

		public IActionResult Details(int id)
		{
			var product = unitOfWork.ProductRepository
				.Get(x => x.Id == id, includeProperties: nameof(Category));
			return View(product);
		}

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