using Microsoft.AspNetCore.Http;
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
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ResetPassword(string id, string password)
		{
			var user = await GetCurrentUser();
			if (user == null) return NotFound();
			
			await _userManager.ResetPasswordAsync(user, "", password);

			return Ok();
		}



		[HttpDelete]
		public async Task<IActionResult> DeleteAccount()
		{
			var user = await GetCurrentUser();
			if (user == null) return RedirectToAction("Login", "Home");
			await _userManager.DeleteAsync(user);
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateProfile(string email)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("An Error Occured. Please Try Again");
			}
			else
			{
				var user = await GetCurrentUser();
				user.Email = email;
				await _userManager.UpdateAsync(user);

				return Ok();
			}
		}

		
	}
}
