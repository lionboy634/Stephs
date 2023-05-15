using Microsoft.AspNetCore.Mvc;

namespace Stephs_Shop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AdminController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult ManageUser()
		{
			return View();
		}
	}
}
