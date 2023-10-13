using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Models.Options;
using Stephs_Shop.Repositories;
using Stephs_Shop.Services;
using System;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace Stephs_Shop.Controllers
{
	//[Authorize]
	public class CustomerController : BaseController
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly ICartRepository cartRepository;
		private readonly IProductRepository _productRepository;
		private readonly ILogger<CustomerController> _logger;
		private readonly IEmailSender _emailSender;
		private readonly HttpClient _client;
		private readonly ITransactionRepository _transactionRepository;
		private readonly MicroServiceOption _microServiceOption;
		public CustomerController(IOrderRepository orderRepository,
			ICartRepository cartRepository,
			IProductRepository productRepository, 
			ILogger<CustomerController> logger,
			IEmailSender emailSender,
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			ICustomerRepository customerRepository,
			ITransactionRepository transactionRepository,
			IOptions<MicroServiceOption> microServiceOption) : base(userManager, signInManager)
		{
			_orderRepository = orderRepository;
			this.cartRepository = cartRepository;
			_productRepository = productRepository;
			_logger = logger;
			_emailSender = emailSender;
			_client = new HttpClient();
			_customerRepository = customerRepository;
			_transactionRepository = transactionRepository;
			_microServiceOption = microServiceOption.Value;
		}
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
				decimal sum = 0m;
				foreach(var item in products)
				{
					var product = await _productRepository.GetProduct(item.id).ConfigureAwait(false);
					if(product == null)
					{
						return NotFound("Product Not Found");
					}
					sum += item.price;
				}
				//decimal sum = product.Select(c=>c.price).Sum();
				if(amount < sum)
				{
					return BadRequest("Amount paid is less the price of products");
				}
				using (var scope = new TransactionScope(TransactionScopeOption.Required , new TransactionOptions{	IsolationLevel = IsolationLevel.ReadUncommitted}, TransactionScopeAsyncFlowOption.Enabled))
				{
					try
					{
						_logger.LogInformation("Transaction In Progress");
						RestClient restClient = new RestClient(_microServiceOption.FlutterWaveUrl);
						var request = new RestRequest(string.Empty, Method.Post);
						request.AddHeader("Authorization", $"Bearer {_microServiceOption.FlutterWaveApiKey}");
						dynamic body = new ExpandoObject();
						body.public_key = "";
						body.tx_ref = "";
						body.amount = sum;
						body.tx_ref = Guid.NewGuid();
						body.redirect_url = new Uri("http://localhost:5000/verify");
						body.customer = new
						{
							email = user.Email,
							phone_number = user.PhoneNumber

						};
						
						RestResponse response = restClient.Execute(request);
						var response_content = JsonConvert.DeserializeObject<RestResponse>(response.Content);
						if (!response_content.IsSuccessStatusCode)
						{
							_logger.LogError($"Error: {response_content.ErrorMessage}");
							return BadRequest($"Error: {response_content.ErrorMessage} ");
						}
						
                            //add orders
                       var order_id = await _orderRepository.AddOrderDetail(user.Id, 0);	
                       await _orderRepository.AddOrderItem();

						//create transaction for payment
						var transactionReference = Guid.NewGuid();
						await _transactionRepository.CreateTransaction(transactionReference, user.Id, order_id).ConfigureAwait(false);


						scope.Complete();
                    }
					catch (Exception ex)
					{
						_logger.LogError($"Error Message: {ex.Message}");
						return BadRequest($"Error Occurred: {ex.Message}");
					}

					return Ok();
				}
			}
			return BadRequest();
			
		}

		[HttpGet]
		public IActionResult Cart()
		{
			ViewData["Title"] = "Cart.Page";
			return View();
		}
	}
}
