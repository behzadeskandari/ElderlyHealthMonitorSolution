using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IDeviceService
    {
        Task<DeviceDto> RegisterDeviceAsync(DeviceDto dto, CancellationToken ct = default);
        Task<DeviceDto?> GetByIdentifierAsync(string identifier, CancellationToken ct = default);
    }
}
