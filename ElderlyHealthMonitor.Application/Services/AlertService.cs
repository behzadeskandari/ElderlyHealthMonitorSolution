using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Application.Interfaces;
using ElderlyHealthMonitor.Domain.Entities;
using ElderlyHealthMonitor.Domain.Repository;
using ElderlyHealthMonitorSolution.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ElderlyHealthMonitor.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IMapper _mapper;
        private readonly IHubContext<AlertHub> _hub;


        public AlertService(IAlertRepository alertRepo, IEventRepository eventRepo, IMapper mapper, IHubContext<AlertHub> hub)
        {
            _alertRepo = alertRepo;
            _eventRepo = eventRepo;
            _mapper = mapper;
            _hub = hub;
        }


        public async Task<AlertDto> CreateAlertAsync(EventDto evDto, Guid? caregiverId = null, CancellationToken ct = default)
        {
            var ev = _mapper.Map<Event>(evDto);
            ev.Id = Guid.NewGuid();
            ev.TimestampUtc = ev.TimestampUtc == default ? DateTime.UtcNow : ev.TimestampUtc;
            await _eventRepo.AddAsync(ev, ct);


            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                EventId = ev.Id,
                ElderlyProfileId = ev.ElderlyProfileId,
                CaregiverId = caregiverId,
                SentAtUtc = DateTime.UtcNow,
                Message = BuildMessage(ev),
                Channel = "push",
                Status = "open"
            };


            await _alertRepo.AddAsync(alert, ct);


            var dto = _mapper.Map<AlertDto>(alert);


            // push to SignalR
            await _hub.Clients.Group(ev.ElderlyProfileId.ToString()).SendAsync("AlertCreated", dto, ct);


            return dto;
        }


        public async Task<bool> AckAlertAsync(Guid alertId, Guid caregiverId, CancellationToken ct = default)
        {
            var alert = await _alertRepo.GetByIdAsync(alertId, ct);
            if (alert == null) return false;
            alert.Status = "acked";
            await _alertRepo.UpdateAsync(alert, ct);


            await _hub.Clients.Group(alert.ElderlyProfileId.ToString()).SendAsync("AlertAcked", new { alertId, caregiverId }, ct);
            return true;
        }


        private string BuildMessage(Event ev)
        {
            return ev.EventType switch
            {
                Domain.Enums.EventType.FallDetected => "Fall detected — please check immediately",
                Domain.Enums.EventType.HeartRateAnomaly => "Heart-rate anomaly detected",
                _ => "Event detected"
            };
        }
    }
}
