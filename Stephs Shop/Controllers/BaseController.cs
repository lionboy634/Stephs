using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stephs_Shop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public BaseController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<User> GetCurrentUser()
        {
            var current_user = await _userManager.GetUserAsync(HttpContext.User);
            return current_user;
        }

       public string GenerateTransactionReference(string transactionType)
        {
            var date = new DateTime().Date;
            var txnType = "";
            switch (transactionType)
            {
                case "payment":
                    txnType = "PAY";
                    break;
                case "reversal":
                    txnType = "REV";
                    break;
                default:
                    throw new Exception("Type not Available");
            }

            return $"0000-{txnType}-{date}";
        } 

        private enum TransactionType
        {
            payment,
            reversal

        }

        private enum SortBy
        {
            ASC,
            DESC
        }
       
    }
}
