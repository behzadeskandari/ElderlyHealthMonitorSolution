using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;
using ElderlyHealthMonitor.Domain.Repository;
using ElderlyHealthMonitor.Infrastructure.Data;

namespace ElderlyHealthMonitor.Infrastructure.Repository
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AppDbContext _db;
        public AlertRepository(AppDbContext db) { _db = db; }
        public async Task<Alert> AddAsync(Alert alert, CancellationToken ct = default)
        {
            await _db.Alerts.AddAsync(alert, ct);
            await _db.SaveChangesAsync(ct);
            return alert;
        }


        public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Alerts.FindAsync(new object[] { id }, ct);


        public async Task UpdateAsync(Alert alert, CancellationToken ct = default)
        {
            _db.Alerts.Update(alert);
            await _db.SaveChangesAsync(ct);
        }
    }
}
