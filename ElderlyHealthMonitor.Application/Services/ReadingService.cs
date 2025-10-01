using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Domain.Entities;
using Newtonsoft.Json;

namespace ElderlyHealthMonitor.Application.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IReadingRepository _readingRepo;
        private readonly IDeviceRepository _deviceRepo;
        private readonly IMLService _mlService;
        private readonly IEventRepository _eventRepo;
        private readonly IMapper _mapper;


        public ReadingService(IReadingRepository readingRepo, IDeviceRepository deviceRepo, IMLService mlService, IEventRepository eventRepo, IMapper mapper)
        {
            _readingRepo = readingRepo;
            _deviceRepo = deviceRepo;
            _mlService = mlService;
            _eventRepo = eventRepo;
            _mapper = mapper;
        }


        public async Task<int> IngestReadingsAsync(IEnumerable<SensorReadingDto> readings, CancellationToken ct = default)
        {
            var entities = readings.Select(r => new SensorReading
            {
                Id = Guid.NewGuid(),
                DeviceId = r.DeviceId,
                ElderlyProfileId = r.ElderlyProfileId,
                SensorType = r.SensorType,
                TimestampUtc = r.TimestampUtc == default ? DateTime.UtcNow : r.TimestampUtc,
                ValueDouble = r.Value,
                Unit = r.Unit,
                PayloadJson = r.PayloadJson,
                Processed = false
            }).ToList();


            await _readingRepo.BulkInsertAsync(entities, ct);


            // Basic synchronous checks: heart rate
            var hrReadings = readings.Where(r => r.SensorType == "hr" && r.Value.HasValue).ToList();
            foreach (var hr in hrReadings)
            {
                if (hr.Value > 120 || hr.Value < 40)
                {
                    var ev = new Domain.Entities.Event
                    {
                        Id = Guid.NewGuid(),
                        ElderlyProfileId = hr.ElderlyProfileId,
                        EventType = Domain.Enums.EventType.HeartRateAnomaly,
                        Source = "edge",
                        Severity = Domain.Enums.AlertSeverity.High,
                        TimestampUtc = hr.TimestampUtc,
                        DetailsJson = JsonSerializer.Serialize(new { hr = hr.Value })
                    };
                    await _eventRepo.AddAsync(ev, ct);
                }
            }


            // For falls: group by device and use ML quick detect
            var accWindows = readings.Where(r => r.SensorType == "acc").GroupBy(r => r.DeviceId);
            foreach (var g in accWindows)
            {
                var window = g.ToList();
                var result = await _mlService.DetectFallAsync(window, ct);
                if (result.IsFall)
                {
                    var ev = new Domain.Entities.Event
                    {
                        Id = Guid.NewGuid(),
                        ElderlyProfileId = window.First().ElderlyProfileId,
                        EventType = Domain.Enums.EventType.FallDetected,
                        Source = "ml",
                        TimestampUtc = window.Max(x => x.TimestampUtc),
                        Severity = Domain.Enums.AlertSeverity.Critical,
                        DetailsJson = JsonSerializer.Serialize(new { confidence = result.Confidence })
                    };
                    await _eventRepo.AddAsync(ev, ct);
                }
            }


            return entities.Count;
        }
    }
}
