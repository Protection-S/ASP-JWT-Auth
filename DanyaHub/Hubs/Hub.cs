using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class UserStatusHub : Hub
{
    public async Task SendUserStatus(string username, bool isOnline)
    {
        await Clients.All.SendAsync("ReceiveUserStatus", username, isOnline);
    }
}
