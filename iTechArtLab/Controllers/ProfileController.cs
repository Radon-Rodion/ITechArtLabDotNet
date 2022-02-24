using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Text;
using BuisnessLayer.JWToken;
using BuisnessLayer;
using DataAccessLayer.Entities;

namespace iTechArtLab.Controllers
{
    [Route("/api/user")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly JWTokenConfig _jWTokenConfig;

        public ProfileController(UserManager<User> userManager, IOptions<JWTokenConfig> jWTokenOptions)
        {
            _userManager = userManager;
            _jWTokenConfig = jWTokenOptions.Value;
        }

        /// <summary>
        /// Get request for profile information page
        /// </summary>
        /// <remarks>/api/user</remarks>
        /// <response code="200">View ProfileInfo with fields containing signed in profile information</response>
        /// <response code="401">Sign in required</response>
        [HttpGet]
        public async Task<object> ProfileInfo()
        {
            string errorMessage;
            if (!AccessControlManager.IsTokenValid(HttpContext, out errorMessage)) return errorMessage;

            var user = await _userManager.FindByIdAsync(SessionManager.GetUserId(HttpContext.Session).ToString());
            ProfileViewModel model = new ProfileViewModel() { UserName = user.UserName, Email = user.Email, Delivery = user.Delivery, PhoneNumber = user.PhoneNumber };
            return View(model);
        }

        /// <summary>
        /// Put request for profile info update
        /// </summary>
        /// <remarks>/api/user</remarks>
        /// <param name="email" example="email11example@tut.by">New email</param>
        /// <param name="userName" example="User009">New username</param>
        /// <param name="delivery" example="Minsk, Dzerginskogo, 95/112">New delivery address</param>
        /// <param name="phoneNumber" example="+7125678990">New phone number</param>
        /// <response code="200">New(updated) profile info</response>
        /// <response code="400">Errors list</response>
        /// <response code="401">Sign in required</response>
        [HttpPut]
        public async Task<object> ProfileInfo(string email, string userName, string delivery, string phoneNumber)
        {
            string errorMessage;
            if (!AccessControlManager.IsTokenValid(HttpContext, out errorMessage)) return errorMessage;

            var model = new ProfileViewModel() { UserName = userName, Email = email, Delivery = delivery, PhoneNumber = phoneNumber };
            var modelErrors = ModelValidator.ValidateViewModel(model);

            if (modelErrors.Count == 0)
            {
                var user = await _userManager.FindByIdAsync(SessionManager.GetUserId(HttpContext.Session).ToString());
                user.Email = email;
                user.UserName = userName;
                user.PhoneNumber = phoneNumber;
                user.Delivery = delivery;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    SessionManager.SetToken(HttpContext.Session, JWTokenGenerator.GenerateToken(user?.Email, userRole, _jWTokenConfig));
                    return Json(new { email = user.Email, userName = user.UserName, delivery = user.Delivery, phoneNumber = user.PhoneNumber });
                }
                else
                {
                    StringBuilder errors = new StringBuilder("");
                    foreach (var error in result.Errors)
                    {
                        Log.Logger.Warning($"{error.Description} happened at {DateTime.UtcNow.ToLongTimeString()}");
                        errors.Append(error.Description).Append('\n');
                    }
                    HttpContext.Response.StatusCode = 400;
                    return errors.ToString();
                }
            }
            HttpContext.Response.StatusCode = 400;
            return ModelValidator.StringifyErrors(modelErrors);
        }

        /// <summary>
        /// Get request for password change page
        /// </summary>
        /// <remarks>/api/user/password</remarks>
        /// <response code="200">View PasswordChange</response>
        /// <response code="401">Sign in required</response>
        [HttpGet("password")]
        public object PasswordChange()
        {
            string errorMessage;
            if (!AccessControlManager.IsTokenValid(HttpContext, out errorMessage)) return errorMessage;
            return View();
        }

        /// <summary>
        /// Patch request for password changing
        /// </summary>
        /// <remarks>/api/user/password</remarks>
        /// <param name="oldPassword" example="_123456OldPassword">Actual password of the account</param>
        /// <param name="newPassword" example="_123456NewPassword">New password for the account</param>
        /// <param name="newPasswordConfirm" example="_123456NewPassword">New password confirmation for the account. Must be equal to newPassword</param>
        /// <response code="204">New password set, return "{}"</response>
        /// <response code="400">Errors list</response>
        /// <response code="401">Sign in required</response>
        [HttpPatch("password")]
        public async Task<object> PatchPasswordChange(string oldPassword, string newPassword, string newPasswordConfirm)
        {
            string errorMessage;
            if (!AccessControlManager.IsTokenValid(HttpContext, out errorMessage)) return errorMessage;

            var model = new ChangePasswordViewModel() { OldPassword = oldPassword, NewPassword = newPassword, NewPasswordConfirm = newPasswordConfirm };
            var modelErrors = ModelValidator.ValidateViewModel(model);

            if (modelErrors.Count == 0)
            {
                var user = await _userManager.FindByIdAsync(SessionManager.GetUserId(HttpContext.Session).ToString());

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    HttpContext.Response.StatusCode = 204;
                    return "{}";
                }
                else
                {
                    StringBuilder errors = new StringBuilder("");
                    foreach (var error in result.Errors)
                    {
                        Log.Logger.Warning($"{error.Description} happened at {DateTime.UtcNow.ToLongTimeString()}");
                        errors.Append(error.Description).Append('\n');
                        //ModelState.AddModelError(string.Empty, error.Description);
                    }
                    HttpContext.Response.StatusCode = 400;
                    return errors.ToString();
                }
            }
            HttpContext.Response.StatusCode = 400;
            return ModelValidator.StringifyErrors(modelErrors);
        }
    }
}