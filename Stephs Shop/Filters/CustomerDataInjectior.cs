using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Stephs_Shop.Models;
using Stephs_Shop.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.NewFolder
{
	public class CustomerDataInjectior : IAsyncActionFilter
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly IConnectionMultiplexer _connectionMultiplexer;
		private readonly ILogger<CustomerDataInjectior> _logger;
		public CustomerDataInjectior(
			ILogger<CustomerDataInjectior> logger,
			ICustomerRepository customerRepository,
			IConnectionMultiplexer connectionMultiplexer)
		{
			_customerRepository = customerRepository;
			_connectionMultiplexer = connectionMultiplexer;
			_logger = logger;

		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var redis = _connectionMultiplexer.GetDatabase( db : -1);
			var customerData = await redis.StringGetAsync("customers");
			var customerOrders = await redis.StringGetAsync("customer_order");

			if (!customerData.HasValue)
			{
				_logger.LogInformation("Customer Data already Cached");
				var customers = await _customerRepository.GetAllCustomers();
				await redis.StringSetAsync("customers", JsonConvert.SerializeObject(customers));
			}
			if (!customerOrders.HasValue)
			{

			}

			var result = await next();
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			throw new System.NotImplementedException();
		}

		

		
	}
}
