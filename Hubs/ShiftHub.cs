using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TimeGhazi.Hubs
{
    public class ShiftHub : Hub
    {
        public async Task NotifyShiftUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveShiftUpdate", message);
        }
    }
}