using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElderlyHealthMonitor.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ElderlyProfile> ElderlyProfiles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<MedicationSchedule> MedicationSchedules { get; set; }
        public DbSet<BehavioralPattern> BehavioralPatterns { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<ElderlyProfile>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.ElderlyProfiles)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Device>()
                .HasOne(d => d.ElderlyProfile)
                .WithMany(ep => ep.Devices)
                .HasForeignKey(d => d.ElderlyProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SensorReading>()
                .HasOne(sr => sr.Device)
                .WithMany(d => d.SensorReadings)
                .HasForeignKey(sr => sr.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SensorReading>()
                .HasOne(sr => sr.ElderlyProfile)
                .WithMany(ep => ep.SensorReadings)
                .HasForeignKey(sr => sr.ElderlyProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.ElderlyProfile)
                .WithMany()
                .HasForeignKey(e => e.ElderlyProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Event)
                .WithOne(e => e.Alert)
                .HasForeignKey<Alert>(a => a.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.ElderlyProfile)
                .WithMany()
                .HasForeignKey(a => a.ElderlyProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.User)
                .WithMany(u => u.Alerts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicationSchedule>()
                .HasOne(ms => ms.ElderlyProfile)
                .WithMany(ep => ep.MedicationSchedules)
                .HasForeignKey(ms => ms.ElderlyProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BehavioralPattern>()
                .HasOne(bp => bp.ElderlyProfile)
                .WithMany(ep => ep.BehavioralPatterns)
                .HasForeignKey(bp => bp.ElderlyProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            modelBuilder.Entity<SensorReading>()
                .HasIndex(sr => new { sr.ElderlyProfileId, sr.TimestampUtc })
                .HasMethod("btree") // For range queries
                .IsDescending(false, true); // DESC on timestamp for latest first

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.Identifier)
                .IsUnique();

            modelBuilder.Entity<Event>()
                .HasIndex(e => new { e.EventType, e.TimestampUtc });

            modelBuilder.Entity<Alert>()
                .HasIndex(a => a.SentAtUtc);
        }
    }
}
