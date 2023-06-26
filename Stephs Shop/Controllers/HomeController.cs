using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Stephs_Shop.Areas.Admin.Models;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Repositories;
using Stephs_Shop.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public HomeController(ILogger<HomeController> logger, 
            SignInManager<User> signInManager,
            UserManager<User> userManager, ICustomerRepository customerRepository,
            ISmsSender smsSender,
            IEmailSender emailSender,
            IProductRepository productRepository,
            IConnectionMultiplexer connectionMultiplexer) : base(userManager, signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _smsSender = smsSender;
            _productRepository = productRepository;
            _emailSender = emailSender;
            _connectionMultiplexer = connectionMultiplexer;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index(int page_number = 1)
        {
            ViewData["Title"] = "Home.Page";
            var product_count = (await _productRepository.GetAllProduct().ConfigureAwait(false)).Count();
            int limit = 20;
           
            var page = (int)product_count / limit;
            var offset = (page_number - 1) * limit;

            var products = await _productRepository.GetProducts().ConfigureAwait(false);
            var results = new PaginatedResult<Product>
            {
                Items = products.ToArray(),
                PageCount = products.Count()
            };
            var justArrivedProducts = products.Where(c=> (c.created_at - DateTime.Now).Days < 5).ToList();  
            var product_category = await _productRepository.GetProductCategory();
            var Mencategory = await _productRepository.GetProductSubCategory("men").ConfigureAwait(false);
            var WomenCategory = await _productRepository.GetProductSubCategory("Women").ConfigureAwait(false);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            ViewData["Title"] = "Cart.Page";
            var current_user = await GetCurrentUser();
            if(current_user == null) return RedirectToAction(nameof(Login), "Home");

            

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewData["Title"] = "Registration.Page";
            var currentuser = await GetCurrentUser();
            if(currentuser != null)
            {
                return RedirectToAction(nameof(Index));
            }
           
            return View();
        }


        [HttpPost]    
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var existinguser = await _userManager.FindByEmailAsync(model.Email);
                if(existinguser != null)
                {
                    ModelState.AddModelError("", "User already exists");
                    return View();
                }
                else
                {
                    var user = new User
                    {
                        FirstName = model.FirstName.Trim(), 
                        LastName = model.LastName.Trim(),
                        Email = model.Email.Trim(),
                        PhoneNumber = model.Mobile.Trim(),
                        UserName = model.Email.Trim()
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        if (user.Email.Contains("admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Dashboard", "Office", new {area = "admin"} );
                        }
                        await _userManager.AddToRoleAsync(user, "Member");
                        await _customerRepository.CreateUserAsync(user);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                         return RedirectToAction("Step2", "Home"); 
                    }
                    else
                    {
                        foreach(IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Step2()
        {
            var user = await GetCurrentUser();
            if(user == null)
            {
                return RedirectToAction(nameof(Register), "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> step2(string addressline1, string addressline2, string mobile_number)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUser();
                var found =  await _userManager.FindByEmailAsync(user.Email);
                if(found == null)
                {
                   return RedirectToAction(nameof(Index), "Home");
                }
                var address = new Address
                {
                    Addressline1 = addressline1.Trim(),
                    Addressline2 = addressline2.Trim()
                };
               await _customerRepository.UpdateAddress(user.Id, address).ConfigureAwait(false);
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login), "Home");
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var current_user = await GetCurrentUser();
            if(current_user != null)
                RedirectToAction(nameof(Index), "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password, bool RememberMe=true)
        {
            if (ModelState.IsValid)
            {
                var current_user = await GetCurrentUser();
                if(current_user != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var user = await _userManager.FindByEmailAsync(Email);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email or Password is invalid");
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, false);
                if (result.Succeeded)
                {
                    var session = new CustomerSession
                    {
                        Id = Guid.NewGuid().ToString(),
                        Customer = user.Id,
                        Total = 1,
                        CreatedAt = DateTime.Now
                    };
                    //await _customer.CreateShoppingSession(session);
                    var member_role = await _userManager.IsInRoleAsync(user, "Member");
                    if (await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        return RedirectToAction("Home", "Customer");
                    }
                    else if(await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Dashboard", "Office", new { area = "admin" });
                        //return RedirectToAction("");
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", "");
                    }
                  
                }
                else if (result.RequiresTwoFactor)
                {
                    

                }
                else if (result.IsLockedOut)
                {

                }
                else
                {
                    return View();
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Contact(string contact)
        {
            var currentuser = await GetCurrentUser();
            if(currentuser == null)
            {
               return RedirectToAction(nameof(Login));
            }
            _logger.LogDebug($"Contact Update for {currentuser.Id}");
            await _customerRepository.UpdateContact(currentuser.Id, contact).ConfigureAwait(false);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContact(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUser();
                if(user == null)
                {
                    return RedirectToAction(nameof(Login));
                }
                await _customerRepository.UpdateContact(user.Id, "");
                return RedirectToAction(nameof(Index));
            }
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> SearchProducts(string product, int limit = 50)
        {
            var products = await _productRepository.GetProducts();
            int product_count = products.Count();
            int page = product_count / limit;

            var results = new PaginatedResult<Product>();
            results.Items = products.ToArray();
            results.PageCount = page;
            results.PageCount = product_count;
            return View("Product", results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
