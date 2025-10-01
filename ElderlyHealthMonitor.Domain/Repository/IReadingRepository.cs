using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IReadingRepository
    {
        Task BulkInsertAsync(IEnumerable<SensorReading> readings, CancellationToken ct = default);
        Task<IEnumerable<SensorReading>> GetRecentByElderlyIdAsync(Guid elderlyId, DateTime sinceUtc, CancellationToken ct = default);
    }
}
