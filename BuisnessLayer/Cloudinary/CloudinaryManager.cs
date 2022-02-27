using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Serilog;

namespace BuisnessLayer.Cloudinary
{
    public class CloudinaryManager
    {
        private Account account;
        private CloudinaryDotNet.Cloudinary cloudinary;

        public CloudinaryManager(string apiKey, string apiSecret, string cloudName)
        {
            account = new Account { ApiKey = apiKey, ApiSecret = apiSecret, Cloud = cloudName };
            cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public string UploadImage(string imageFilePath)
        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFilePath)
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                return uploadResult.Url.AbsoluteUri;
            } catch(Exception e)
            {
                Log.Logger.Error(e.Message);
                return "";
            }
        }
    }
}
