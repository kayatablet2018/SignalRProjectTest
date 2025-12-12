using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRApi.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            // Bağlanan istemci hakkında ek loglama veya işlemler burada yapılabilir.
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Bağlantısı kesilen istemci hakkında ek loglama veya işlemler burada yapılabilir.
            await base.OnDisconnectedAsync(exception);
        }
    }
}
