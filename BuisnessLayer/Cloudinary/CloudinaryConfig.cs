using System;
using System.Collections.Generic;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BuisnessLayer.Cloudinary
{
    public class CloudinaryConfig
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string CloudName {get; set;}
    }
}
