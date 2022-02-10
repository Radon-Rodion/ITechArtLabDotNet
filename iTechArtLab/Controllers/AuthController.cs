using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Text;
using BuisnessLayer.JWToken;
using BuisnessLayer.Senders;
using BuisnessLayer;

namespace iTechArtLab.Controllers
{
    [Route("/api/auth")]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AuthController(RoleManager<IdentityRole<int>> roleManager, UserManager<IdentityUser<int>> userManager, SignInManager<IdentityUser<int>> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            DBInitializer.InitializeAsync(_userManager, _roleManager).Wait();
        }

        [HttpGet]
        public async Task<string> Index() //shows all accs info
        {
            if (!SessionManager.HasToken(HttpContext.Session)) //check authenticated
            {
                HttpContext.Response.StatusCode = 401;
                return "Sign in first!";
            }

            var result = new StringBuilder($"Email : EmailConfirmed : Is admin\n");
            foreach (var user in _userManager.Users) result.Append($"{user.Email} : {user.EmailConfirmed} : {(await _userManager.GetRolesAsync(user)).Any(e => e== "admin")}\n");
            return result.ToString();
        }

        [HttpGet("sign-up")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost("sign-up")]
        public async Task<object> SignUp(SignUpViewModel model) //performs registration
        {
            if (ModelState.IsValid)
            {
                IdentityUser<int> user = new IdentityUser<int> { Email = model.Email, UserName = model.Email };
                // adding user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    SendConfirmationEmailAsync(user);
                    HttpContext.Response.StatusCode = 201;
                    return "To finish registration check your mailbox and follow the link in confirmation email";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Log.Logger.Warning($"{error.Description} happened at {DateTime.UtcNow.ToLongTimeString()}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    HttpContext.Response.StatusCode = 400;
                }
            }
            return View(model);
        }

        [NonAction]
        private async void SendConfirmationEmailAsync(IdentityUser<int> user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://{Request.Host}/api/auth/email-confirmation?id={user.Id}&token={emailConfirmationToken}";
            IEmailSender sender = new SmtpSender();
            sender.SendMess(user.Email, "Confirm email for account", $"To confirm this email, please visit link: {confirmationLink}");
        }

        [HttpGet("email-confirmation")]
        public async Task<string> ConfirmAcc(string id, string token)
        {
            HttpContext.Response.StatusCode = 400;

            if (id == null) return "User id required!";
            if (token == null) return "Email confirmation token required!";

            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return "Invalid user id!";
            var confirmationResult = await _userManager.ConfirmEmailAsync(user, token.Replace(' ','+'));

            if (!confirmationResult.Succeeded) {
                StringBuilder errors = new StringBuilder("");
                foreach (var error in confirmationResult.Errors) errors.Append(error.Description).Append('\n');
                SendConfirmationEmailAsync(user);
                return errors.ToString();
            }

            var settingRoleResult = await _userManager.AddToRoleAsync(user, "user");
            if (!settingRoleResult.Succeeded)
            {
                StringBuilder errors = new StringBuilder("");
                foreach (var error in settingRoleResult.Errors) errors.Append(error.Description).Append('\n');
                SendConfirmationEmailAsync(user);
                return errors.ToString();
            }

            HttpContext.Response.StatusCode = 204;
            return "Account successfully confirmed!";
        }

        [HttpGet("sign-in")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost("sign-in")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model) //performs authentication
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    SessionManager.SetToken(HttpContext.Session, JWTokenGenerator.GenerateToken(user?.Email, userRole));
                    return Ok($"Glad to see you, {user.Email} !");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login/password!");
                    HttpContext.Response.StatusCode = 401;
                }
            }
            return View(model);
        }
    }
}
