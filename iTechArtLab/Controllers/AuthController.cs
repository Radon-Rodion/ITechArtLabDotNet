using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
        private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SmtpConfig _smtpConfig;
        private readonly JWTokenConfig _jWTokenConfig;

        public AuthController(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager, IOptions<SmtpConfig> smtpOptions, IOptions<JWTokenConfig> jWTokenOptions)
        {
            _userManager = userManager;
            //_signInManager = signInManager;
            _roleManager = roleManager;
            _smtpConfig = smtpOptions.Value;
            _jWTokenConfig = jWTokenOptions.Value;
            DBInitializer.InitializeAsync(_userManager, _roleManager).Wait();
        }

        /// <summary>
        /// Get request for all profiles info
        /// </summary>
        /// <remarks>/api/auth</remarks>
        /// <response code="200">Stringifyed table with all accounts info</response>
        /// <response code="401">Sign in required</response>
        [HttpGet]
        public async Task<string> Index() //shows all accs info
        {
            string errorMessage;
            if (!AccessControlManager.IsTokenValid(HttpContext, out errorMessage)) return errorMessage;

            var result = new StringBuilder($"Email : UserName : EmailConfirmed : Is admin : PasswordHash\n");
            foreach (var user in _userManager.Users) result.Append($"{user.Email} : {user.UserName} : {user.EmailConfirmed} : {(await _userManager.GetRolesAsync(user)).Any(e => e== "admin")} : {user.PasswordHash}\n");
            return result.ToString();
        }

        /// <summary>
        /// Get request for sign-up page
        /// </summary>
        /// <remarks>/api/auth/sign-up</remarks>
        /// <response code="200">SignUp View</response>
        [HttpGet("sign-up")]
        public IActionResult SignUp()
        {
            return View();
        }

        /// <summary>
        /// Post request to sign-up new account
        /// </summary>
        /// <remarks>/api/auth/sign-up</remarks>
        /// <response code="201">new account is created but still not activated</response>
        /// <response code="400">View SignUp with errors messages</response>
        [HttpPost("sign-up")]
        public async Task<object> SignUp(SignUpViewModel model) //performs registration
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email };
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

        /// <summary>
        /// email-sending method with auto-generated information to confirm email box
        /// </summary>
        /// <param name="user">user that will get email</param>
        /// <remarks>Private</remarks>
        [NonAction]
        private async void SendConfirmationEmailAsync(User user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://{Request.Host}/api/auth/email-confirmation?id={user.Id}&token={emailConfirmationToken}";
            IEmailSender sender = new SmtpSender(_smtpConfig);
            sender.SendMess(user.Email, "Confirm email for account", $"To confirm this email, please visit link: {confirmationLink}");
        }

        /// <summary>
        /// Get request for email activation of new account
        /// </summary>
        /// <remarks>/api/auth/email-confirmation</remarks>
        /// <param name="id" example="1342">confirming account id</param>
        /// <param name="token" example="AAbdfkle95k2ls+91DgsS67jd">email confirmation token</param>
        /// <response code="204">Account activated and message about activation is returned</response>
        /// <response code="400">Error message</response>
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

        /// <summary>
        /// Get request for sign-in page
        /// </summary>
        /// <remarks>/api/auth/sign-in</remarks>
        /// <response code="200">SignIn View</response>
        [HttpGet("sign-in")]
        public IActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Post request to sign-in account
        /// </summary>
        /// <remarks>/api/auth/sign-in</remarks>
        /// <response code="200">JWT tolen is set into session and greeting message is returned</response>
        /// <response code="401">View SignIn with errors messages</response>
        [HttpPost("sign-in")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model) //performs authentication
        {
            if (ModelState.IsValid)
            {
                var hasher = new PasswordHasher<User>();
                var user = await _userManager.FindByEmailAsync(model.Email);
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false); result.Succeeded

                if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("", "Invalid login/password!");
                    HttpContext.Response.StatusCode = 401;
                }
                else
                {
                    var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    SessionManager.SetToken(HttpContext.Session, JWTokenGenerator.GenerateToken(user?.Email, userRole, _jWTokenConfig));
                    SessionManager.SetUserId(HttpContext.Session, user.Id);
                    return Ok($"Glad to see you, {user.UserName} !");
                }
            }
            return View(model);
        }
    }
}
