using System;
using Xunit;
using BuisnessLayer;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace TestProject.BuisnessLogicTests
{
    public class SessionManagerUnitTest
    {
        MoqSession moqSession = new MoqSession();
        const string MOQ_TOKEN = "MoqJWToken12_0 ";

        [Fact]
        public void TestSetting()
        {
            Assert.Throws<NullReferenceException>(() => SessionManager.SetToken(null, MOQ_TOKEN));
            Assert.Throws<ArgumentNullException>(() => SessionManager.SetToken(moqSession, null));

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

        private class MoqSession : ISession
        {
            IDictionary<string, byte[]> sessionDict;

            public MoqSession() => sessionDict = new Dictionary<string, byte[]>();

            public bool IsAvailable => sessionDict != null;

            public string Id => throw new NotImplementedException();

            public IEnumerable<string> Keys => sessionDict.Keys;

            public void Clear()
            {
                sessionDict.Clear();
            }

            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task LoadAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public void Remove(string key)
            {
                sessionDict.Remove(key);
            }

            public void Set(string key, byte[] value)
            {
                if (sessionDict.ContainsKey(key)) sessionDict.Remove(key);
                sessionDict.Add(new KeyValuePair<string, byte[]>(key, value));
            }

            public bool TryGetValue(string key, out byte[] value)
            {
                return sessionDict.TryGetValue(key, out value);
            }
        }
    }
}
