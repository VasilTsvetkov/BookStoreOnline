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
		private readonly ApplicationDbContext db;
		private readonly UserManager<IdentityUser> userManager;

		public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
		{
			this.db = db;
			this.userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult RoleManagement(string userId)
		{
			var roleId = db.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
			var viewModel = new RoleManagementViewModel();
			viewModel.ApplicationUser = db.ApplicationUsers
				.Include(x => x.Company).FirstOrDefault(x => x.Id == userId);

			viewModel.Roles = db.Roles.Select(x => new SelectListItem
			{
				Text = x.Name, 
				Value = x.Id.ToString()
			});

			viewModel.Companies = db.Companies.Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = x.Id.ToString()
			});

			viewModel.ApplicationUser.Role = db.Roles.FirstOrDefault(x => x.Id == roleId).Name;
			return View(viewModel);
		}

		[HttpPost]
		public IActionResult RoleManagement(RoleManagementViewModel viewModel)
		{
			var roleId = db.UserRoles.FirstOrDefault(x => x.UserId == viewModel.ApplicationUser.Id).RoleId;
			var oldRole = db.Roles.FirstOrDefault(x => x.Id == roleId).Name;

			if (viewModel.ApplicationUser.Role != oldRole)
			{
				var user = db.ApplicationUsers.FirstOrDefault(x => x.Id == viewModel.ApplicationUser.Id);
				
				if (viewModel.ApplicationUser.Role == GlobalConstants.RoleCompany) 
				{ 
					user.CompanyId = viewModel.ApplicationUser.CompanyId;
				}
				if (oldRole == GlobalConstants.RoleCompany)
				{
					user.CompanyId = null;
				}

				db.SaveChanges();

				userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
				userManager.AddToRoleAsync(user, viewModel.ApplicationUser.Role).GetAwaiter().GetResult();
			}
			
			return RedirectToAction(nameof(Index));
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			var users = db.ApplicationUsers.Include(x => x.Company).ToList();

			var userRoles = db.UserRoles.ToList();
			var roles = db.Roles.ToList();

			foreach (var item in users)
			{
				var roleId = userRoles.FirstOrDefault(x => x.UserId == item.Id).RoleId;
				item.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;

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
			var user = db.ApplicationUsers.FirstOrDefault(x => x.Id == id);

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

			db.SaveChanges();

			return Json(new { success = true, message = "Operation is successfull." });
		}

		#endregion
	}
}
