using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IReadingService
    {
        Task<int> IngestReadingsAsync(IEnumerable<SensorReadingDto> readings, CancellationToken ct = default);
    }
}
