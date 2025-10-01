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
    public class DeviceRepository : IDeviceRepository
    {
        private readonly AppDbContext _db;
        public DeviceRepository(AppDbContext db) { _db = db; }
        public async Task AddAsync(Device device, CancellationToken ct = default)
        {
            await _db.Devices.AddAsync(device, ct);
            await _db.SaveChangesAsync(ct);
        }
        public async Task<Device?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Devices.FindAsync(new object[] { id }, ct);
        public async Task<Device?> GetByIdentifierAsync(string identifier, CancellationToken ct = default)
        => await _db.Devices.FirstOrDefaultAsync(d => d.Identifier == identifier, ct);
        public async Task UpdateAsync(Device device, CancellationToken ct = default)
        {
            _db.Devices.Update(device);
            await _db.SaveChangesAsync(ct);
        }
    }
}
