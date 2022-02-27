using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BuisnessLayer;

using DataAccessLayer.Models;

namespace TestProject.DataAcessLayerTests
{
    public class ViewModelsUnitTest
    {
        ModelValidator modelValidator = new ModelValidator();

        [Fact]
        public void SignInViewModelPositiveTest()
        {
            var validModel = new SignInViewModel
            {
                Email = "some00email@gmail.com",
                Password="Qwerty123"
            };
            var errorcount = modelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void SignInViewModelNegativeTest()
        {
            var modelWithout2Fields = new SignInViewModel
            {
                RememberMe = true
            };
            var errorcount = modelValidator.ValidateViewModel(modelWithout2Fields).Count();
            Assert.Equal(2, errorcount);
        }

        [Fact]
        public void SignUpViewModelPositiveTest()
        {
            var validModel = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123",
                PasswordConfirm = "Qwerty123"
            };
            var errorcount = modelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void SignUpViewModelNegativeTest()
        {
            var modelWithout1Field = new SignUpViewModel
            {
                Email = "some00email@gmail.com",
                Password = "Qwerty123"
            };
            var errorcount = modelValidator.ValidateViewModel(modelWithout1Field).Count();
            Assert.Equal(1, errorcount);
        }

        [Fact]
        public void ProfileInfoViewModelPositiveTest()
        {
            var validModel = new ProfileViewModel
            {
                Email = "some00email@gmail.com",
                UserName = "Arkadyj",
                Delivery = "Homel, Sovetskaya 14/21"
            };
            var errorcount = modelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void ProfileInfoViewModelNegativeTest()
        {
            var modelWithout1Field = new ProfileViewModel
            {
                Email = "some00email@gmail.com",
                PhoneNumber = "+375336512314"
            };
            var errorcount = modelValidator.ValidateViewModel(modelWithout1Field).Count();
            Assert.Equal(1, errorcount);

            var modelWithout2Fields = new ProfileViewModel
            {
                Delivery = "Homel, Sovetskaya 14/21",
                PhoneNumber = "+375336512314"
            };

            errorcount = modelValidator.ValidateViewModel(modelWithout2Fields).Count();
            Assert.Equal(2, errorcount);
        }

        [Fact]
        public void PasswordChangeViewModelPositiveTest()
        {
            var validModel = new ChangePasswordViewModel
            {
                OldPassword = "123456_Test",
                NewPassword = "_Test123456",
                NewPasswordConfirm = "_Test123456",
            };
            var errorcount = modelValidator.ValidateViewModel(validModel).Count();
            Assert.Equal(0, errorcount);
        }

        [Fact]
        public void PasswordChangeViewModelNegativeTest()
        {
            var modelWithout1Field = new ChangePasswordViewModel
            {
                OldPassword = "123456_Test",
                NewPassword = "_Test123456",
            };
            var errorcount = modelValidator.ValidateViewModel(modelWithout1Field).Count();
            Assert.Equal(1, errorcount);

            var modelWithout2Fields = new ChangePasswordViewModel
            {
                NewPassword = "_Test123456",
            };

            errorcount = modelValidator.ValidateViewModel(modelWithout2Fields).Count();
            Assert.Equal(2, errorcount);
        }
    }
}
