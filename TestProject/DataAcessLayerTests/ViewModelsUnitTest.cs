using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using DataAccessLayer.Models;

namespace TestProject.DataAcessLayerTests
{
    public class ViewModelsUnitTest
    {
        [Fact]
        public void SignInViewModelTest()
        {
            var validModel = new SignInViewModel
            {
                Email = "some00email@gmail.com",
                Password="Qwerty123"
            };
            var errorcount = ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);

            var modelWithout2Fields = new SignInViewModel
            {
                RememberMe=true
            };
            errorcount = ValidateViewModel(modelWithout2Fields).Count();
            Assert.Equal(2, errorcount);
        }

        [Fact]
        public void SignUpViewModelTest()
        {
            var validModel = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123",
                PasswordConfirm = "Qwerty123"
            };
            var errorcount = ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);

            var modelWithout1Field = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123"
            };
            errorcount = ValidateViewModel(modelWithout1Field).Count();
            Assert.Equal(1, errorcount);
        }

        public IList<ValidationResult> ValidateViewModel(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

            return result;
        }
    }
}
