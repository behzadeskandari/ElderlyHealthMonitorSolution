using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IElderlyRepository
    {
        Task<ElderlyProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ElderlyProfile>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(ElderlyProfile entity, CancellationToken ct = default);
        Task UpdateAsync(ElderlyProfile entity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
