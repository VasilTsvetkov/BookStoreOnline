using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreOnlineWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductsController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IWebHostEnvironment webHostEnvironment;

		public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			this.unitOfWork = unitOfWork;
			this.webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			var products = unitOfWork.ProductRepository.GetAll(includeProperties: nameof(Category)).ToList();

			return View(products);
		}

		public IActionResult Upsert(int? id)
		{
			var viewModel = new ProductViewModel();
			viewModel.CategoryList = unitOfWork.CategoryRepository
			   .GetAll().Select(x => new SelectListItem
			   {
				   Text = x.Name,
				   Value = x.Id.ToString(),
			   });
			viewModel.Product = new Product();

			if (id == null || id == 0)
			{
				return View(viewModel);
			}
			else
			{
				viewModel.Product = unitOfWork.ProductRepository.Get(x => x.Id == id);
				return View(viewModel);
			}
		}

		[HttpPost]
		public IActionResult Upsert(ProductViewModel viewModel, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(viewModel.Product.ImageUrl))
					{
						var oldImagePath =
							Path.Combine(wwwRootPath, viewModel.Product.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					viewModel.Product.ImageUrl = @"\images\product\" + fileName;
				}

				if (viewModel.Product.Id == 0)
				{
					unitOfWork.ProductRepository.Add(viewModel.Product);
					TempData["success"] = "Product created successfully.";
				}
				else
				{
					unitOfWork.ProductRepository.Update(viewModel.Product);
					TempData["success"] = "Product updated successfully.";
				}
				unitOfWork.Save();

				return RedirectToAction(nameof(Index));
			}
			else
			{
				viewModel.CategoryList = unitOfWork.CategoryRepository
				   .GetAll().Select(x => new SelectListItem
				   {
					   Text = x.Name,
					   Value = x.Id.ToString(),
				   });

				return View(viewModel);
			}
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			var products = unitOfWork.ProductRepository
				.GetAll(includeProperties: nameof(Category)).ToList();
			return Json(new { data = products });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var product = unitOfWork.ProductRepository.Get(x => x.Id == id);

			if (product == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			unitOfWork.ProductRepository.Remove(product);
			unitOfWork.Save();

			return Json(new { success = true, message = "Product deleted successfully." });
		}
		#endregion
	}
}
