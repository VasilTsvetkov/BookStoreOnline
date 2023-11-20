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
	public class CompaniesController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		public CompaniesController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var companies = unitOfWork.CompanyRepository.GetAll().ToList();

			return View(companies);
		}

		public IActionResult Upsert(int? id)
		{
			if (id == null || id == 0)
			{
				return View(new Company());
			}
			else
			{
				Company company = unitOfWork.CompanyRepository.Get(x => x.Id == id);
				return View(company);
			}
		}

		[HttpPost]
		public IActionResult Upsert(Company company)
		{
			if (ModelState.IsValid)
			{
				if (company.Id == 0)
				{
					unitOfWork.CompanyRepository.Add(company);
					TempData["success"] = "Company created successfully.";
				}
				else
				{
					unitOfWork.CompanyRepository.Update(company);
					TempData["success"] = "Company updated successfully.";
				}
				unitOfWork.Save();

				return RedirectToAction(nameof(Index));
			}
			else
			{				
				return View(company);
			}
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			var companies = unitOfWork.CompanyRepository.GetAll().ToList();
			return Json(new { data = companies });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var company = unitOfWork.CompanyRepository.Get(x => x.Id == id);

			if (company == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			unitOfWork.CompanyRepository.Remove(company);
			unitOfWork.Save();

			return Json(new { success = true, message = "Company deleted successfully." });
		}
		#endregion
	}
}
