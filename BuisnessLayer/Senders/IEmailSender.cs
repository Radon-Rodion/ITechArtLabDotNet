using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Senders
{
    public interface IEmailSender
    {
        public void SendMess(string addressTo, string messageTheme, string messageBody);
    }
}
