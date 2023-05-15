using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Services;
using System;
using System.Threading.Tasks;

namespace Stephs_Shop.Controllers
{
	public class RecoveryController : BaseController
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ISmsSender _smsSender;
		private readonly IEmailSender _emailSender;
		public RecoveryController(UserManager<User> userManager, 
			SignInManager<User> signInManager,
			IEmailSender emailSender, ISmsSender smsSender) : base(userManager, signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_smsSender = smsSender;
		}

		public async Task<IActionResult> RecoverPassword(string Email)
		{
			var user = await _userManager.FindByEmailAsync(Email);
			if (user == null)
			{
				return BadRequest("Email Doesnt Exists");
			}
			Uri recoverpasswordUrl = new Uri($"http://localhost:5000/recovery/resetpassword/{user.Id}");
			EmailMessage emailMessage = new EmailMessage();
			emailMessage.subject = "Password Recovery";
			emailMessage.To.Add(Email);
			emailMessage.message = recoverpasswordUrl.ToString();
			_emailSender.SendEmail(emailMessage);
			return View();
		}



		[HttpGet]
		public async Task<IActionResult> ResetPassword(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if(user == null)
			{
				return BadRequest("User doesnt exists");
			}
			var userModel = new
			{
				email = user.Email
			};

			return View(userModel);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(string id, string password)
		{
			var user = await GetCurrentUser();
			
			await _userManager.ResetPasswordAsync(user, "", password);

			return Ok();
		}



		[HttpPost]
		public async Task<IActionResult> DeleteAccount()
		{
			var user = await GetCurrentUser();

			await _userManager.DeleteAsync(user);

			return RedirectToAction(nameof(Index));
		}


		
	}
}
