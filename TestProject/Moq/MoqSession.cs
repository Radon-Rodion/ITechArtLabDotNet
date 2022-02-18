using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TestProject.Moq
{
    class MoqSession : ISession
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
