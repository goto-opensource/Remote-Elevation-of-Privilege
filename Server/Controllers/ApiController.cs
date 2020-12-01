using Microsoft.AspNetCore.Mvc;

namespace rEoP.Server.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {
        private readonly ISessionStore _store;

        public ApiController(ISessionStore store) : base()
        {
            _store = store;
        }

        [HttpPost]
        [Route("api/whiteboardUpload")]
        public StatusCodeResult WhiteboardUpload([FromBody] string content)
        {
            try
            {
                var v = content.IndexOf("/");
                var sessionIdHash = content.Substring(0, v);
                if (_store.Sessions.TryGetValue(sessionIdHash, out var sess))
                {
                    sess.Whiteboard ??= content[(v + 1)..];
                    return new StatusCodeResult(200);
                }
            }
            catch
            {
            }
            return new StatusCodeResult(500);
        }

        [HttpPost]
        [Route("api/whiteboardDownload")]
        public string WhiteboardDownload([FromBody] string content)
        {
            try
            {
                if (_store.Sessions.TryGetValue(content, out var sess))
                {
                    return sess.Whiteboard ?? "STAHP";
                }
            }
            catch
            {
            }
            return "STAHP";
        }
    }
}
