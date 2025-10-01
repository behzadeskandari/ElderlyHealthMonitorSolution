using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface IMLService
    {
        Task<AnomalyResult> DetectBehavioralAnomalyAsync(Guid elderlyId, IEnumerable<SensorReading> readings);
        Task<FallResult> DetectFallAsync(SensorReadingWindow window);
        Task<HeartRateResult> DetectHeartRateAnomalyAsync(double hr);
    }
}
