using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using BookStoreOnline.Models.ViewModels;
using BookStoreOnline.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStoreOnlineWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = GlobalConstants.RoleAdmin)]
	public class UsersController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly UserManager<IdentityUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;

		public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			this.unitOfWork = unitOfWork;
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult RoleManagement(string userId)
		{
			var viewModel = new RoleManagementViewModel();
			viewModel.ApplicationUser = unitOfWork.ApplicationUserRepository
				.Get(x => x.Id == userId, includeProperties: nameof(Company));

			viewModel.Roles = roleManager.Roles.Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = x.Name
			});

			viewModel.Companies = unitOfWork.CompanyRepository.GetAll().Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = x.Id.ToString()
			});

			viewModel.ApplicationUser.Role = userManager.GetRolesAsync(unitOfWork.ApplicationUserRepository.Get(x => x.Id == userId))
				.GetAwaiter().GetResult().FirstOrDefault();
			return View(viewModel);
		}

		[HttpPost]
		public IActionResult RoleManagement(RoleManagementViewModel viewModel)
		{
			var oldRole = userManager.GetRolesAsync(unitOfWork.ApplicationUserRepository.Get(x => x.Id == viewModel.ApplicationUser.Id))
				.GetAwaiter().GetResult().FirstOrDefault();

			var user = unitOfWork.ApplicationUserRepository.Get(x => x.Id == viewModel.ApplicationUser.Id);

			if (viewModel.ApplicationUser.Role != oldRole)
			{
				if (viewModel.ApplicationUser.Role == GlobalConstants.RoleCompany)
				{
					user.CompanyId = viewModel.ApplicationUser.CompanyId;
				}
				if (oldRole == GlobalConstants.RoleCompany)
				{
					user.CompanyId = null;
				}
				unitOfWork.ApplicationUserRepository.Update(user);
				unitOfWork.Save();

				userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
				userManager.AddToRoleAsync(user, viewModel.ApplicationUser.Role).GetAwaiter().GetResult();
			}
			else
			{
				if (oldRole == GlobalConstants.RoleCompany && user.CompanyId != viewModel.ApplicationUser.CompanyId)
				{
					user.CompanyId = viewModel.ApplicationUser.CompanyId;
					unitOfWork.ApplicationUserRepository.Update(user);
					unitOfWork.Save();
				}
			}

			return RedirectToAction(nameof(Index));
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			var users = unitOfWork.ApplicationUserRepository.GetAll(includeProperties: nameof(Company));

			foreach (var item in users)
			{
				item.Role = userManager.GetRolesAsync(item).GetAwaiter().GetResult().FirstOrDefault();

				if (item.Company == null)
				{
					item.Company = new Company();
					item.Company.Name = "";
				}
			}

			return Json(new { data = users });
		}

		[HttpPost]
		public IActionResult LockUnlock([FromBody] string id)
		{
			var user = unitOfWork.ApplicationUserRepository.Get(x => x.Id == id);

			if (user == null)
			{
				return Json(new { success = false, message = "Error ocurred while locking/unlocking." });
			}

			if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
			{
				user.LockoutEnd = DateTime.UtcNow;
			}
			else
			{
				user.LockoutEnd = DateTime.UtcNow.AddYears(100);
			}

			unitOfWork.ApplicationUserRepository.Update(user);
			unitOfWork.Save();

			return Json(new { success = true, message = "Operation is successfull." });
		}

		#endregion
	}
}
