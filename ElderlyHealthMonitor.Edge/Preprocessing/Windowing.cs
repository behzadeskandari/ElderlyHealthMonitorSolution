using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.DTOS.DTO;

namespace ElderlyHealthMonitor.Edge.Preprocessing
{
    public static class Windowing
    {
        // Create sliding windows of sensor readings grouped by device / elderly if needed
        public static IEnumerable<IEnumerable<SensorReadingDto>> SlidingWindows(
            IEnumerable<SensorReadingDto> samples,
            int windowSize = 32,
            int step = 16)
        {
            var list = samples.OrderBy(s => s.TimestampUtc).ToList();
            for (int i = 0; i + windowSize <= list.Count; i += step)
            {
                yield return list.Skip(i).Take(windowSize);
            }
        }

        // Build single window from last N samples
        public static IEnumerable<SensorReadingDto> LastWindow(IEnumerable<SensorReadingDto> samples, int windowSize = 32)
        {
            var list = samples.OrderBy(s => s.TimestampUtc).ToList();
            return list.Skip(Math.Max(0, list.Count - windowSize)).Take(windowSize);
        }
    }
}
