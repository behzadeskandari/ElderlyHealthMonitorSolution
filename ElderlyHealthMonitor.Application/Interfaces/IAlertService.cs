using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IAlertService
    {
        Task<Alert> CreateAlertAsync(Event ev, Guid caregiverId);
        Task<bool> AckAlertAsync(Guid alertId, Guid caregiverId);
    }
}
