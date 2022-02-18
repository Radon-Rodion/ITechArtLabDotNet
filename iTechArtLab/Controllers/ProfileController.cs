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

        [HttpGet]
        public async Task<object> ProfileInfo()
        {
            string errorMessage;
            if (!AccessControlManager.ValidateAccess(HttpContext, out errorMessage)) return errorMessage;

            var user = await _userManager.FindByIdAsync(SessionManager.GetUserId(HttpContext.Session).ToString());
            ProfileViewModel model = new ProfileViewModel() { UserName = user.UserName, Email = user.Email, Delivery=user.Delivery, PhoneNumber = user.PhoneNumber };
            return View(model);
        }

        [HttpPut]
        public async Task<object> ProfileInfo(string email, string userName, string delivery, string phoneNumber)
        {
            string errorMessage;
            if (!AccessControlManager.ValidateAccess(HttpContext, out errorMessage)) return errorMessage;

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
                    return Json(new { email= user.Email, userName= user.UserName, delivery = user.Delivery, phoneNumber= user.PhoneNumber });
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

        [HttpGet("password")]
        public object PasswordChange()
        {
            string errorMessage;
            if (!AccessControlManager.ValidateAccess(HttpContext, out errorMessage)) return errorMessage;
            return View();
        }

        [HttpPatch("password")]
        public async Task<object> PatchPasswordChange(string oldPassword, string newPassword, string newPasswordConfirm)
        {
            string errorMessage;
            if (!AccessControlManager.ValidateAccess(HttpContext, out errorMessage)) return errorMessage;

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
