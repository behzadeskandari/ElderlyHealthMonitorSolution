using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Application.Interfaces
{
    public interface ISensorIngest
    {
        void SaveReadings(List<SensorReadingDto> SensorReadingDtolst);

        void Preprocess(SensorReadingWindow SensorReadingWindow);

        void DetectFall(SensorReadingWindow SensorReadingWindow);

        void DetectHeartRateAnomaly(double hr);

        void EmitEvent(EventType, ElderlyId, Severity);
    }
}
