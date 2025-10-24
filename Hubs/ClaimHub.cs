using Microsoft.AspNetCore.SignalR;

namespace ContractMonthlyClaimSystemPOE.Hubs
{
    public class ClaimHub : Hub
    {
        public async Task NotifyClaimUpdate(string userId, string status)
        {
            await Clients.User(userId).SendAsync("ClaimUpdated", status);
        }
    }
}