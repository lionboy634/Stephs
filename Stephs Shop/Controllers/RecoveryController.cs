using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stephs_Shop.Controllers
{
	public class RecoveryController : BaseController
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ISmsSender _smsSender;
		private readonly IEmailSender _emailSender;
		private readonly ILogger _logger;
		public RecoveryController(UserManager<User> userManager, 
			SignInManager<User> signInManager,
			IEmailSender emailSender,
			ISmsSender smsSender,
			ILogger logger) : base(userManager, signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_smsSender = smsSender;
			_logger = logger;
		}

		public async Task<IActionResult> RecoverPassword(string Email)
		{
			var user = await _userManager.FindByEmailAsync(Email);
			if (user == null)
			{
				return BadRequest("Email Doesnt Exists");
			}
			Uri recoverpasswordUrl = new Uri($"http://localhost:5000/recovery/resetpassword/{user.Id}");
			EmailSender.EmailMessage emailMessage = new EmailSender.EmailMessage();
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
		public async Task<IActionResult> ResetPassword(User user)
		{
			try
			{
				var foundUser = await _userManager.FindByEmailAsync(user.Email);
				if (foundUser == null)
				{
					return NotFound("User Does not Exist");
				}
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var message = new EmailSender.EmailMessage
				{
					from = "",
					message = token,
					subject = "Password Reset Confirmation"
				};
				message.To.Add(foundUser.Email);
				_emailSender.SendEmail(message);
				return RedirectToAction();

			}
			catch(Exception ex)
			{
				_logger.LogError($"Error: {ex.Message}");
				return BadRequest("Error: Something Went Wrong");
			}
			
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
