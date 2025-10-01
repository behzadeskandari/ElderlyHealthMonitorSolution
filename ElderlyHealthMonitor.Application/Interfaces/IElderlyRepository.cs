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
        Task<ElderlyProfile> GetByIdAsync(Guid id);
        Task AddAsync(ElderlyProfile elderly);
        Task<IEnumerable<ElderlyProfile>> GetAllAsync();
    }
}
