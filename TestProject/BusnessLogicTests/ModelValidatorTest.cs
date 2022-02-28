using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BuisnessLayer;
using DataAccessLayer.Models;

namespace TestProject.BusnessLogicTests
{
    public class ModelValidatorTest
    {
        ModelValidator modelValidator = new ModelValidator();

        [Fact]
        public void ValidatorPositiveTest()
        {
            object model = new SignInViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123"
            };
            var errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);

            model = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123",
                PasswordConfirm = "Qwerty123",
            };
            errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);

            model = new ProfileViewModel
            {
                Email = "some00email@gmail.com",
                UserName = "User111"
            };
            errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);

            model = new ChangePasswordViewModel
            {
                OldPassword = "OldPassword_",
                NewPassword = "Qwerty123",
                NewPasswordConfirm = "Qwerty123",
            };
            errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);
        }

        [Fact]
        public void ValidatorNegativeTest()
        {
            object model = new SignInViewModel
            {
                Email = "some00email@gmail.com"
            };
            var errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);

            model = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123"
            };
            errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);

            model = new ProfileViewModel
            {
                Delivery = "Homel, Sovetskaya 14/21",
                PhoneNumber = "+375336512314"
            };
            errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);

            model = new ChangePasswordViewModel
            {
                NewPassword = "Qwerty123",
                NewPasswordConfirm = "Qwerty123",
            };
            errors = ValidateViewModel(model);
            Assert.Equal(modelValidator.ValidateViewModel(model).Count, errors.Count);
        }

        [Fact]
        public void ValidatorStringifyTest()
        {
            object model = new SignInViewModel
            {
                Email = "some00email@gmail.com"
            };
            Assert.Equal("Password: The Password field is required.\n", modelValidator.StringifyErrors(modelValidator.ValidateViewModel(model)));

            model = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123"
            };
            var errors = ValidateViewModel(model);
            Assert.Equal("PasswordConfirm: The Confirm password field is required.\n", modelValidator.StringifyErrors(modelValidator.ValidateViewModel(model)));

            model = new ProfileViewModel
            {
                Delivery = "Homel, Sovetskaya 14/21",
                PhoneNumber = "+375336512314"
            };
            errors = ValidateViewModel(model);
            Assert.Equal("UserName: The User name field is required.\nEmail: The Email field is required.\n",
                modelValidator.StringifyErrors(modelValidator.ValidateViewModel(model)));

            model = new ChangePasswordViewModel
            {
                NewPassword = "Qwerty123",
                NewPasswordConfirm = "Qwerty123",
            };
            errors = ValidateViewModel(model);
            Assert.Equal("OldPassword: The Old password field is required.\n", modelValidator.StringifyErrors(modelValidator.ValidateViewModel(model)));
        }

        private IList<ValidationResult> ValidateViewModel(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

            return result;
        }
    }
}
