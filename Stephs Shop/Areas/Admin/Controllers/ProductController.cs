using Microsoft.AspNetCore.Mvc;

namespace Stephs_Shop.Areas.Admin.Controllers
{
	public class ProductController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
