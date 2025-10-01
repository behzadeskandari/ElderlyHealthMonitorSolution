using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace ElderlyHealthMonitorSolution.API.Hubs
{
    public class AlertHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            // clients should join group by elderlyId to receive alerts for that person
            return base.OnConnectedAsync();
        }


        public Task JoinElderGroup(string elderlyId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, elderlyId);
        }


        public Task LeaveElderGroup(string elderlyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, elderlyId);
        }
    }
}
