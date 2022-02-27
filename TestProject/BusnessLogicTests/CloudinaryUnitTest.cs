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
            var uri = cloudinaryManager.UploadImage("p:\\xbox.png");
            Assert.Contains("http://res.cloudinary.com/radon-rodion/image/upload/", uri);
        }
    }
}
