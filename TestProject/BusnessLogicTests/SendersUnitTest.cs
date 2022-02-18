using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BuisnessLayer.Senders;
using TestProject.Moq;

namespace TestProject.BuisnessLogicTests
{
    public class SendersUnitTest
    {
        [Fact]
        public void TestSmtp()
        {
            IEmailSender mailer = new SmtpSender(MoqConfigs.smtpConfig);
            mailer.SendMess("radon0rodion@gmail.com", "UnitTest message", "This is just a program test");
        }
    }
}
