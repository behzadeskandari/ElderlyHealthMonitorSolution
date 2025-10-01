using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IAlertService
    {
        Task<AlertDto> CreateAlertAsync(EventDto evDto, Guid? caregiverId = null, CancellationToken ct = default);
        Task<bool> AckAlertAsync(Guid alertId, Guid caregiverId, CancellationToken ct = default);
    }
}
