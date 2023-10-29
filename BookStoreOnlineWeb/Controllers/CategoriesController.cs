using BookStoreOnlineWeb.Data;
using BookStoreOnlineWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreOnlineWeb.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly ApplicationDbContext db;

		public CategoriesController(ApplicationDbContext db)
		{
			this.db = db;
		}

		public IActionResult Index()
		{
			var categories = db.Categories.ToList();
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
				db.Categories.Add(category);
				db.SaveChanges();
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

			Category? category = db.Categories.Find(id);

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
				db.Categories.Update(category);
				db.SaveChanges();
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

			Category? category = db.Categories.Find(id);

			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Category? category = db.Categories.Find(id);

			if (category == null)
			{
				return NotFound();
			}

			db.Categories.Remove(category);
			db.SaveChanges();
			TempData["success"] = "Category deleted successfully.";
			return RedirectToAction(nameof(Index));
		}
	}
}
