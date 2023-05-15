using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Repositories;
using Stephs_Shop.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace Stephs_Shop.Controllers
{
	public class CustomerController : BaseController
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly ICartRepository cartRepository;
		private readonly IProductRepository _productRepository;
		private readonly ILogger<CustomerController> _logger;
		private readonly IEmailSender _emailSender;
		private readonly HttpClient _client;
		public CustomerController(IOrderRepository orderRepository,
			ICartRepository cartRepository,
			IProductRepository productRepository, 
			ILogger<CustomerController> logger,
			IEmailSender emailSender,
			UserManager<User> userManager,
			SignInManager<User> signInManager) : base(userManager, signInManager)
		{
			_orderRepository = orderRepository;
			this.cartRepository = cartRepository;
			_productRepository = productRepository;
			_logger = logger;
			_emailSender = emailSender;
			_client = new HttpClient();
		}
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> MakeTransaction(Product[] products, decimal amount, int quantity, decimal price)
		{
			//do checks for parameter
			if(ModelState.IsValid)
			{
				var user = await GetCurrentUser();
				if(quantity < 0)
				{
					return BadRequest("Quantity must be more than zero");
				}
				decimal sum = 0;
				foreach(var item in products)
				{
					
					var product = await _productRepository.GetProduct(item.id).ConfigureAwait(false);
					//remove this
					if(product == null)
					{
						return BadRequest("Product doesnt exists");
					}
					sum += item.price;

				}
				//decimal sum = product.Select(c=>c.price).Sum();
				if(amount < sum)
				{
					return BadRequest("Amount paid is less the price of products");
				}
				
				using (var transaction = new TransactionScope())
				{
					try
					{
						RestClient restClient = new RestClient();
						//make payment 

						//add cart




						using(TransactionScope transactionScope = new TransactionScope())
						{
                            //add orders
                            var order_id = await _orderRepository.AddOrderDetail(user.Id, 0);	
                            await _orderRepository.AddOrderItem();

                            //create transaction for payment



                            //


                        }


                    }
					catch (Exception ex)
					{
						_logger.LogCritical($"Error Message: {ex.Message}");
						throw new Exception(ex.Message);              

					}

					return StatusCode(200, new {});
				}
			}

			return BadRequest();
			
			

		}
	}
}
