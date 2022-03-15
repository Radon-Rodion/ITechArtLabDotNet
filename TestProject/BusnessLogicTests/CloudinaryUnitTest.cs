using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer.Cloudinary;

namespace TestProject.BusnessLogicTests
{
    public class CloudinaryUnitTest
    {
        [Fact]
        public void UploadImagePositiveTest()
        {
            var cloudinaryManager = new CloudinaryManager("912279616246254", "ipI0v7g-_jfkl-zRXZXEjSzZ4Do", "radon-rodion");
            var uri = cloudinaryManager.UploadImage("https://besplatnye-programmy.com/uploads/posts/2019-04/1554187375_microsoft_picture_manager.jpg");
            Assert.Contains("http://res.cloudinary.com/radon-rodion/image/upload/", uri);
        }
    }
}
