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
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace iTechArtLab.Controllers
{
    [Route("/api/user")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly JWTokenConfig _jWTokenConfig;
        private readonly ModelValidator _modelValidator;
        private readonly AccessControlManager _accessControlManager;
        private readonly SessionManager _sessionManager;
        private readonly JWTokenGenerator _jWTokenGenerator;
        private readonly JWTokenValidator _jWTokenValidator;
        private readonly IMemoryCache _memoryCache;

        public ProfileController(UserManager<User> userManager, IOptions<JWTokenConfig> jWTokenOptions, SessionManager sessionManager, IMemoryCache memoryCache,
            JWTokenValidator jWTokenValidator, JWTokenGenerator jWTokenGenerator, AccessControlManager accessControlManager, ModelValidator modelValidator)
        {
            _userManager = userManager;
            _jWTokenConfig = jWTokenOptions.Value;

            _modelValidator = modelValidator;
            _sessionManager = sessionManager;
            _jWTokenValidator = jWTokenValidator;
            _jWTokenGenerator = jWTokenGenerator;
            _accessControlManager = accessControlManager;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get request for profile information page
        /// </summary>
        /// <remarks>/api/user</remarks>
        /// <response code="200">View ProfileInfo with fields containing signed in profile information</response>
        /// <response code="401">Sign in required</response>
        [HttpGet]
        public async Task<object> GetProfileInfo()
        {
            string errorMessage;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) return errorMessage;

            User user = null;
            int id = _sessionManager.GetUserId(HttpContext.Session);
            if (!_memoryCache.TryGetValue(id, out user))
            {
                user = await _userManager.FindByIdAsync(id.ToString());
                _memoryCache.Set(id, user);
            }

            ProfileViewModel model = new ProfileViewModel() { UserName = user.UserName, Email = user.Email, Delivery = user.Delivery, PhoneNumber = user.PhoneNumber };
            return View("ProfileInfo",model);
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
        public async Task<object> PutProfileInfo(string email, string userName, string delivery, string phoneNumber)
        {
            string errorMessage;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) return errorMessage;

            var model = new ProfileViewModel() { UserName = userName, Email = email, Delivery = delivery, PhoneNumber = phoneNumber };
            var modelErrors = _modelValidator.ValidateViewModel(model);

            if (modelErrors.Count == 0)
            {
                
                int id = _sessionManager.GetUserId(HttpContext.Session);
                User user = user = await _userManager.FindByIdAsync(id.ToString());
                _memoryCache.Remove(id);

                user.Email = email;
                user.UserName = userName;
                user.PhoneNumber = phoneNumber;
                user.Delivery = delivery;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    _sessionManager.SetToken(HttpContext.Session, _jWTokenGenerator.GenerateToken(user?.Email, userRole, _jWTokenConfig));
                    return Json(new { email = user.Email, userName = user.UserName, delivery = user.Delivery, phoneNumber = user.PhoneNumber }, 
                        new JsonSerializerOptions() { IgnoreNullValues = true });
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
            return _modelValidator.StringifyErrors(modelErrors);
        }

        /// <summary>
        /// Get request for password change page
        /// </summary>
        /// <remarks>/api/user/password</remarks>
        /// <response code="200">View PasswordChange</response>
        /// <response code="401">Sign in required</response>
        [HttpGet("password")]
        public object GetPasswordChangeForm()
        {
            string errorMessage;
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) return errorMessage;
            return View("PasswordChange");
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
            if (!_accessControlManager.IsTokenValid(HttpContext, out errorMessage, _sessionManager)) return errorMessage;

            var model = new ChangePasswordViewModel() { OldPassword = oldPassword, NewPassword = newPassword, NewPasswordConfirm = newPasswordConfirm };
            var modelErrors = _modelValidator.ValidateViewModel(model);

            if (modelErrors.Count == 0)
            {
                User user = null;
                int id = _sessionManager.GetUserId(HttpContext.Session);
                if (!_memoryCache.TryGetValue(id, out user))
                {
                    user = await _userManager.FindByIdAsync(id.ToString());
                }
                else _memoryCache.Remove(id);

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
            return _modelValidator.StringifyErrors(modelErrors);
        }
    }
}