using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Juga.Mvc.Providers;

/// <summary>
/// SignalR connection id'leri userId ile eşleştirmeye yarar.
/// Bu sayede kullanıcıya özel bir mesaj gönderirken userId bilgisi ile mesaj gönderilebilir.
/// </summary>
public class UserIdProvider : IUserIdProvider
{
    private const string UserIdClaimType = "preferred_username";
    public string GetUserId(HubConnectionContext connection)
    {
        var name = connection.User.FindFirstValue(UserIdClaimType);
        return name;
    }
}