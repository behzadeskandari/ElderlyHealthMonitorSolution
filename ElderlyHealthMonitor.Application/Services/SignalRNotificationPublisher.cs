using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitorSolution.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ElderlyHealthMonitor.Application.Services
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<AlertHub> _hub;
        public SignalRNotificationPublisher(IHubContext<AlertHub> hub) { _hub = hub; }

        public Task PublishAlertAsync(object payload, string group) =>
            _hub.Clients.Group(group).SendAsync("AlertCreated", payload);

        public Task PublishAlertAckAsync(Guid alertId, Guid caregiverId, string group) =>
            _hub.Clients.Group(group).SendAsync("AlertAcked", new { alertId, caregiverId });
    }
}
