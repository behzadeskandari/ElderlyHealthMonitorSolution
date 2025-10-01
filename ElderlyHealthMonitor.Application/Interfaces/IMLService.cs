using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Application.Records;
using ElderlyHealthMonitor.Domain.Entities;
using ElderlyHealthMonitor.DTOS.DTO;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IMLService
    {
        Task<AnomalyResult> DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReadingDto> readings, CancellationToken ct = default);
        Task<FallResult> DetectFallAsync(IEnumerable<SensorReadingDto> window, CancellationToken ct = default);
    }
}
