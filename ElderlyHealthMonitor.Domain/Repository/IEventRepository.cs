using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IEventRepository
    {
        Task AddAsync(Event ev, CancellationToken ct = default);
    }
}
