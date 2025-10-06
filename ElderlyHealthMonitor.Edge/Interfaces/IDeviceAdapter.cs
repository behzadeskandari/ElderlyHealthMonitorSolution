using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Edge.Models;

namespace ElderlyHealthMonitor.Edge.Interfaces
{
    public interface IDeviceAdapter : IDisposable
    {
        /// <summary>
        /// شروع اتصال و خواندن داده‌ها از دستگاه
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// رویدادی که وقتی داده جدید آمد فراخوانی می‌شود
        /// </summary>
        event EventHandler<SensorData> OnSensorData;

        /// <summary>
        /// آیا adapter به دستگاه وصل است
        /// </summary>
        bool IsConnected { get; }
    }
}
