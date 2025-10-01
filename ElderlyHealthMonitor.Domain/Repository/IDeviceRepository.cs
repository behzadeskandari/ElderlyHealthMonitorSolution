using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Device?> GetByIdentifierAsync(string identifier, CancellationToken ct = default);
        Task AddAsync(Device device, CancellationToken ct = default);
        Task UpdateAsync(Device device, CancellationToken ct = default);
    }
}
