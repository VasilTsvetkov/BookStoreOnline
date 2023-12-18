using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Models.ViewModels;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreOnlineWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = GlobalConstants.RoleAdmin)]
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
				viewModel.Product = unitOfWork.ProductRepository.Get(x => x.Id == id, includeProperties: nameof(Product.ProductImages));
				return View(viewModel);
			}
		}

		[HttpPost]
		public IActionResult Upsert(ProductViewModel viewModel, List<IFormFile> files)
		{
			if (ModelState.IsValid)
			{
				if (viewModel.Product.Id == 0)
				{
					unitOfWork.ProductRepository.Add(viewModel.Product);
				}
				else
				{
					unitOfWork.ProductRepository.Update(viewModel.Product);
				}

				unitOfWork.Save();

				string wwwRootPath = webHostEnvironment.WebRootPath;
				if (files != null)
				{
					foreach (var item in files)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
						string productPath = @"images\products\product-" + viewModel.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);

						if (!Directory.Exists(finalPath))
						{
							Directory.CreateDirectory(finalPath);
						}

						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
						{
							item.CopyTo(fileStream);
						}

						var productImage = new ProductImage();
						productImage.ImageUrl = @"\" + productPath + @"\" + fileName;
						productImage.ProductId = viewModel.Product.Id;

						if (viewModel.Product.ProductImages == null)
						{
							viewModel.Product.ProductImages = new List<ProductImage>();
						}

						viewModel.Product.ProductImages.Add(productImage);
					}

					unitOfWork.ProductRepository.Update(viewModel.Product);
					unitOfWork.Save();
				}

				TempData["success"] = "Product created/updated successfully.";
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

		public IActionResult DeleteImage(int imageId)
		{
			var image = unitOfWork.ProductImageRepository.Get(x => x.Id == imageId);
			var productId = image.ProductId;

			if (image != null)
			{
				if (!string.IsNullOrEmpty(image.ImageUrl))
				{
					var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				unitOfWork.ProductImageRepository.Remove(image);
				unitOfWork.Save();

				TempData["success"] = "Image deleted successfully.";
			}

			return RedirectToAction(nameof(Upsert), new { id = productId });
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

			string productPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(webHostEnvironment.WebRootPath, productPath);

			if (Directory.Exists(finalPath))
			{
				var filePaths = Directory.GetFiles(finalPath);

				foreach (var item in filePaths)
				{
					System.IO.File.Delete(item);
				}

				Directory.Delete(finalPath);
			}

			unitOfWork.ProductRepository.Remove(product);
			unitOfWork.Save();

			return Json(new { success = true, message = "Product deleted successfully." });
		}
		#endregion
	}
}
