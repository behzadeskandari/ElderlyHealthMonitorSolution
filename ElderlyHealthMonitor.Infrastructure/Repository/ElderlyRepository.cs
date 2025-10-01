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
    public class ElderlyRepository : IElderlyRepository
    {
        private readonly AppDbContext _db;
        public ElderlyRepository(AppDbContext db) { _db = db; }


        public async Task AddAsync(ElderlyProfile entity, CancellationToken ct = default)
        {
            await _db.ElderlyProfiles.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);
        }


        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var e = await _db.ElderlyProfiles.FindAsync(new object[] { id }, ct);
            if (e != null) { _db.ElderlyProfiles.Remove(e); await _db.SaveChangesAsync(ct); }
        }


        public async Task<IEnumerable<ElderlyProfile>> GetAllAsync(CancellationToken ct = default)
        => await _db.ElderlyProfiles.AsNoTracking().ToListAsync(ct);


        public async Task<ElderlyProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ElderlyProfiles.Include(x => x.Devices).FirstOrDefaultAsync(x => x.Id == id, ct);


        public async Task UpdateAsync(ElderlyProfile entity, CancellationToken ct = default)
        {
            _db.ElderlyProfiles.Update(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
