using Microsoft.AspNetCore.SignalR;

namespace rEoP.Server
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext().Request.Cookies.TryGetValue("UserID", out var userid) ? userid : "";
        }
    }
}