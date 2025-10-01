using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Domain.Enums;
using ElderlyHealthMonitor.DTOS.DTO;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface ISensorIngest
    {
        void SaveReadings(List<SensorReadingDto> SensorReadingDtolst);

        void Preprocess(SensorReadingDto SensorReadingWindow);

        void DetectFall(SensorReadingDto SensorReadingWindow);

        void DetectHeartRateAnomaly(double hr);

        void EmitEvent(EventType eventType,int ElderlyId,AlertSeverity Severity);
    }
}
