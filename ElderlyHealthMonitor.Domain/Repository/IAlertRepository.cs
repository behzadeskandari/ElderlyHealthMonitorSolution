using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Domain.Repository
{
    public interface IAlertRepository
    {
        Task<Alert> AddAsync(Alert alert, CancellationToken ct = default);
        Task<Alert?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(Alert alert, CancellationToken ct = default);
    }
}
