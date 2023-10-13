using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Controllers
{
	public class CommerceController : BaseController
	{
		private readonly IProductRepository _productRepository;
		private readonly ICartRepository _cartRepository;
		private readonly ILogger<CommerceController> _logger;
		public CommerceController(
			UserManager<User> userManager,
			SignInManager<User> _signInManager,
			IProductRepository productRepository,
			ICartRepository cartRepository,
			ILogger<CommerceController> logger) : base(userManager, _signInManager)
		{
			_productRepository = productRepository;
			_cartRepository = cartRepository;
			_logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> AddCart(int productId)
		{
			var customer = await GetCurrentUser();
			if (customer == null)
			{
				return NotFound();
			}
			var sessionId = HttpContext.Session.Id;
			var cart = await _cartRepository.LoadCart(productId, sessionId).ConfigureAwait(false);

			var duplicateProduct = cart.FirstOrDefault(c => c.product.id == productId);
			if (duplicateProduct != null)
			{
				return BadRequest("Product Already Exist in the Cart");
			}

			var cartDetail = new Cart
			{

			};
			await _cartRepository.AddCart(cartDetail, sessionId).ConfigureAwait(false);
			int count = 0;
			HttpContext.Session.SetInt32("cart-count", count + 1);
			return Ok();
		}

		[HttpDelete]
		public async Task<IActionResult> RemoveCart(int productId)
		{
			var customer = await GetCurrentUser();
			if (customer == null)
			{
				return NotFound();
			}

			var sessionId = HttpContext.Session.Id;
			var product = await _productRepository.GetProduct(productId);
			if (product == null)
			{
				return NotFound("Product Not Found");
			}
			var cart = (await _cartRepository.LoadCart(productId, sessionId))?.FirstOrDefault(c => c.product.id == productId);
			if (cart is null)
			{
				_logger.LogError($"Cart Not Found");
				return NotFound("Ops Something Happened");
			}

			await _cartRepository.RemoveCart(cart.id, HttpContext.Session.Id);
			var cartcount = HttpContext.Session.GetInt32("cart-count");
			if (cartcount > 0) HttpContext.Session.SetInt32("cart-count", (int)(cartcount - 1));

			return Ok();
		}

		[HttpGet]
		public IActionResult CartCount()
		{
			var count = new
			{
				count = HttpContext.Session.GetInt32("count") ?? 0
			};
			return Ok(count);
		}
	}
}
