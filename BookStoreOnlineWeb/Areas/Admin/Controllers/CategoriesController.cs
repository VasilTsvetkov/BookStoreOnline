using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreOnlineWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = GlobalConstants.RoleAdmin)]
	public class CategoriesController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		public CategoriesController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var categories = unitOfWork.CategoryRepository.GetAll().ToList();
			return View(categories);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category category)
		{
			if (ModelState.IsValid)
			{
				unitOfWork.CategoryRepository.Add(category);
				unitOfWork.Save();
				TempData["success"] = "Category created successfully.";
				return RedirectToAction(nameof(Index));
			}

			return View();
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Category? category = unitOfWork.CategoryRepository.Get(x => x.Id == id);

			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				unitOfWork.CategoryRepository.Update(category);
				unitOfWork.Save();
				TempData["success"] = "Category updated successfully.";
				return RedirectToAction(nameof(Index));
			}

			return View();
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Category? category = unitOfWork.CategoryRepository.Get(x => x.Id == id);

			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Category? category = unitOfWork.CategoryRepository.Get(x => x.Id == id);

			if (category == null)
			{
				return NotFound();
			}

			unitOfWork.CategoryRepository.Remove(category);
			unitOfWork.Save();
			TempData["success"] = "Category deleted successfully.";
			return RedirectToAction(nameof(Index));
		}
	}
}
