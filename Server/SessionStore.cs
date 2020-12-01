using System.Collections.Concurrent;
using rEoP.Shared.Model;

namespace rEoP.Server
{
    public class SessionStore : ISessionStore
    {
        public ConcurrentDictionary<string, Session> Sessions { get; } = new ConcurrentDictionary<string, Session>();
    }

    public interface ISessionStore
    {
        public ConcurrentDictionary<string, Session> Sessions { get; }
    }
}
