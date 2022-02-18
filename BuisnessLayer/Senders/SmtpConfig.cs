using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Senders
{
    public class SmtpConfig
    {
        public string HostName { get; set; }//smtp.gmail.com
        public int Port { get; set; }//587
        public string UserName { get; set; }//radon0rodion@gmail.com
        public string UserPassword {get; set;}//8334191test
    }
}
