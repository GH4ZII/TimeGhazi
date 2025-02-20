using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeGhazi.Hubs
{
    public class ShiftHub : Hub
    {
        // **Lagrer hvilke grupper (ansatte) hver ConnectionId tilhører**
        private static readonly Dictionary<string, string> UserGroups = new Dictionary<string, string>();

        // **Legger en bruker i en gruppe basert på EmployeeId**
        public async Task JoinShiftGroup(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, employeeId);
                UserGroups[Context.ConnectionId] = employeeId;  // 🔥 Lagre hvem som tilhører hvilken gruppe
                Console.WriteLine($"✅ Bruker {Context.ConnectionId} lagt til gruppe {employeeId}");
            }
        }

        // **Når en bruker kobler fra, fjerner vi dem fra gruppen**
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UserGroups.TryGetValue(Context.ConnectionId, out string? groupName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                UserGroups.Remove(Context.ConnectionId);
                Console.WriteLine($"❌ Bruker {Context.ConnectionId} fjernet fra gruppe {groupName}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}