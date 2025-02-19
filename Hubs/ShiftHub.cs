using Microsoft.AspNetCore.SignalR; // Importerer SignalR for sanntidskommunikasjon
using System.Threading.Tasks; // Brukes for asynkrone operasjoner

namespace TimeGhazi.Hubs
{
    // **Definerer en SignalR-hub for skiftoppdateringer**
    public class ShiftHub : Hub
    {
        // **Metode som sender en sanntidsoppdatering til alle tilkoblede klienter**
        public async Task NotifyShiftUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveShiftUpdate", message); 
            // Sender meldingen til alle klienter som lytter på "ReceiveShiftUpdate"
        }
    }
}