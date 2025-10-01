using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Domain.Entities;
using ElderlyHealthMonitor.Infrastructure.Data;

namespace ElderlyHealthMonitor.Infrastructure.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _db;
        public EventRepository(AppDbContext db) { _db = db; }
        public async Task AddAsync(Event ev, CancellationToken ct = default)
        {
            await _db.Events.AddAsync(ev, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
