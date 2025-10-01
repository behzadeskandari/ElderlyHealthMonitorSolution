using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Domain.Entities;
using ElderlyHealthMonitor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElderlyHealthMonitor.Infrastructure.Repository
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly AppDbContext _db;
        public ReadingRepository(AppDbContext db) { _db = db; }
        public async Task BulkInsertAsync(IEnumerable<SensorReading> readings, CancellationToken ct = default)
        {
            await _db.SensorReadings.AddRangeAsync(readings, ct);
            await _db.SaveChangesAsync(ct);
        }


        public async Task<IEnumerable<SensorReading>> GetRecentByElderlyIdAsync(Guid elderlyId, DateTime sinceUtc, CancellationToken ct = default)
        => await _db.SensorReadings.Where(r => r.ElderlyProfileId == elderlyId && r.TimestampUtc >= sinceUtc).OrderByDescending(r => r.TimestampUtc).ToListAsync(ct);
    }
}
