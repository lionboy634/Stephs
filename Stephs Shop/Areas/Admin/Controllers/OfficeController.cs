using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Newtonsoft.Json;
using Stephs_Shop.Models;
using Stephs_Shop.NewFolder;
using Stephs_Shop.Repositories;
using Stephs_Shop.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[ServiceFilter(typeof(CustomerDataInjectior))]
	[Authorize(Roles = "Admin")]
	public class OfficeController : Controller
	{
		private readonly IProductRepository _productRepository;
		private readonly IReportRepository _reportRepository;
		private readonly IFileService _fileService;
		private readonly ILogger<OfficeController> _logger;

		private readonly ICustomerRepository _customerRepository;
		public OfficeController(IProductRepository productRepository, 
			IReportRepository reportRepository,
			IFileService fileService,
			ICustomerRepository customerRepository,
			ILogger<OfficeController> logger)
		{
			_productRepository = productRepository;
			_reportRepository = reportRepository;
			_fileService= fileService;
			_logger = logger;
			_customerRepository = customerRepository;
		}


		[HttpGet]
		public async Task<IActionResult> Dashboard()
		{
			ViewData["Title"] = "Dashboard.View";
			var dashboardReport = await _reportRepository.GetDashBoardReport().ConfigureAwait(false);
			ViewBag.Dashboard = dashboardReport;
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Product()
		{
			ViewData["Title"] = "Product.View";
			ViewBag.Products = await _productRepository.GetAllProduct().ConfigureAwait(false);
			ViewBag.Category = await _productRepository.GetProductCategory().ConfigureAwait(false);
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> AddProduct(string name, string description, IFormFile image , decimal price)
																																																																																																																																							{
			if (ModelState.IsValid)
			{
				var filename = image.FileName;
				var content = filename.Split(".")[1];
				var contentType = image.ContentType;
				List<Binary> binary = new List<Binary>();
				List<byte[]> filebytes =  new List<byte[]>();
				if (! new[]{ "png", "jpg", "jpeg" }.Contains(content))
				{
					return BadRequest("File type must be jpg or png");
				}
				
				//file must not be greater than 7MB
				if(image.Length < 0 && image.Length > 7340032)
				{
					return BadRequest("Image size must not be more than 3mb");
				}
				using(var filestream = image.OpenReadStream())
				using(MemoryStream ms = new MemoryStream())
				{
					filestream.CopyTo(ms);
					//binary.Add(ms.ToArray());
					filebytes.Add(ms.ToArray());
					Binary fileBinary = new Binary(ms.ToArray(), filename);
					_fileService.uploadFile(fileBinary);
					_logger.LogDebug("File Uploaded Successfully");
				}
				Product product = new Product
				{
					price = price,
					name = name,
					description = description,
				};

				await _productRepository.AddProduct(product).ConfigureAwait(false);
				_logger.LogInformation("Product created successfully");
				return Ok("Product Added successfully");
			}

			return BadRequest();
		}


		public async Task<IActionResult> CustomerView(int limit = 50 , int sortBy = 0, int page = 0)
		{
			ViewData["Title"] = "Customer.View";
			var customers = await _customerRepository.GetAllCustomers().ConfigureAwait(false);
			int customer_count = customers.Count();
			limit = limit > customer_count ? limit : customer_count;
			int offset = (page - 1) * limit;
            ViewBag.page_count = Math.Ceiling((double)customer_count / limit);
			
			ViewBag.Customers = customers;
			return View();
		}



		[HttpPost]
		public async Task<IActionResult> SearchCustomer(string searchTerm)
		{
			ViewBag.Customers = await _customerRepository.FetchCustomers(searchTerm).ConfigureAwait(false);
			return View(nameof(CustomerView));
		}

		public async Task<IActionResult> AdvancedSearch(string address, int customerId, DateTime startDate, DateTime endDate )
		{
			if (ModelState.IsValid)
			{

				return Ok();
			}
			return BadRequest();
		}
		
		

	}
}
