using System;
using Xunit;
using BuisnessLayer;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using TestProject.Moq;

namespace TestProject.BuisnessLogicTests
{
    public class SessionManagerUnitTest
    {
        MoqSession moqSession = new MoqSession();
        const string MOQ_TOKEN = "MoqJWToken12_0 ";

        [Fact]
        public void TestSettingNegative()
        {
            Assert.Throws<NullReferenceException>(() => SessionManager.SetToken(null, MOQ_TOKEN));
            Assert.Throws<ArgumentNullException>(() => SessionManager.SetToken(moqSession, null));
        }

        [Fact]
        public void TestSettingPositive()
        {
            SessionManager.SetToken(moqSession, MOQ_TOKEN);

            var moqSessionKeysEnumerator = moqSession.Keys.GetEnumerator();
            moqSessionKeysEnumerator.MoveNext(); //to get first key in moqSession

            byte[] tokenInSession;

            Assert.True(moqSession.TryGetValue(moqSessionKeysEnumerator.Current, out tokenInSession), "Token string not found in session after setting");
            Assert.True(Encoding.ASCII.GetString(tokenInSession) == MOQ_TOKEN, "Token string in session is invalid");
        }

        [Fact]
        public void TestGetting()
        {
            moqSession.Clear();
            Assert.False(SessionManager.HasToken(moqSession), "Session manager HasToken method finds non-existing token");
            Assert.Throws<ArgumentNullException>(() => SessionManager.GetToken(moqSession));

            SessionManager.SetToken(moqSession, MOQ_TOKEN);
            Assert.True(SessionManager.HasToken(moqSession), "Session manager HasToken method can't find existing token");
            Assert.True(SessionManager.GetToken(moqSession) == MOQ_TOKEN, "Got invalid token from session");
        }
    }
}
