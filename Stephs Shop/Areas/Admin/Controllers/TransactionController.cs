using Microsoft.AspNetCore.Http;
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
		private readonly ITransactionRepository _transactionRepository;
		public TransactionController(IOrderRepository orderRepository, ILogger logger, ITransactionRepository transactionRepository)
		{
			_orderRepository = orderRepository;
			_logger = logger;
			_transactionRepository = transactionRepository;
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



		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> UpdateOrderDeliveryStatus(string transactionReference)
		{
			var order = await _orderRepository.GetOrderById(transactionReference);
			if(order == null)
			{
				return NotFound();
			}
			if(order.Delivered)
			{
				return BadRequest("Order Has Already Been Delivered");
			}
			await _orderRepository.UpdateOrderDeliveryStatus(order.Id);
			return Ok("Order Delivery Status Updated");
		}


		private string GenerateReference(string transactionType)
		{
			DateTime date = new DateTime().Date;

			return $"0000-{date}-{date}";
		}

		
	}
}
