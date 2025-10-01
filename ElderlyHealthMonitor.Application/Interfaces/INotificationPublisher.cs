using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface INotificationPublisher
    {
        Task PublishAlertAsync(object payload, string group); // keep general
        Task PublishAlertAckAsync(Guid alertId, Guid caregiverId, string group);
    }
}
