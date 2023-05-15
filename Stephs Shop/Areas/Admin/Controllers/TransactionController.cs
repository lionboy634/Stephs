using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stephs_Shop.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class TransactionController : Controller
	{
		private readonly IOrderRepository _orderRepository;
		private readonly ILogger _logger;
		public TransactionController(IOrderRepository orderRepository, ILogger logger)
		{
			_orderRepository = orderRepository;
			_logger = logger;

		}


		[HttpGet]
		public async Task<IActionResult> OrderView(int limit = 100, int sortBy = 0, int page = 1)
		{
			ViewData["Title"] = "Orders.View";
			var orders = await _orderRepository.GetAllOrders();
			var order_count = orders.Count();
			page = order_count > limit ? page : limit;



			return View();
		}


		[HttpPost]
		public async Task<IActionResult> ReverseTransaction(string transactionId)
		{
			var transactionReference = GenerateReference("");
			return Ok();
		}



		[HttpPost]
		public async Task<IActionResult> UpdateOrderDeliveryStatus(string transactionReference)
		{
			var order = await _orderRepository.GetOrderById(transactionReference);
			if(order == null)
			{
				return NotFound();
			}
			if(order.Delivered)
			{
				return BadRequest("Order has been delivered. Cannot deliver a delivered object");
			}

			await _orderRepository.UpdateOrderDeliveryStatus(order.Id);
			

			return Ok("Order Delivered");
		}


		private string GenerateReference(string transactionType)
		{
			DateTime date = new DateTime().Date;

			return $"0000-{date}-{date}";
		}

		
	}
}
