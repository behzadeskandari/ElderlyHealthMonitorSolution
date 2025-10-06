using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Edge.Interfaces;
using ElderlyHealthMonitor.Edge.Models;

namespace ElderlyHealthMonitor.Edge.Services
{
    public class BleDeviceManager
    {
        private readonly List<IDeviceAdapter> _adapters = new List<IDeviceAdapter>();

        public void RegisterAdapter(IDeviceAdapter adapter)
        {
            _adapters.Add(adapter);
            adapter.OnSensorData += Adapter_OnSensorData;
        }

        private void Adapter_OnSensorData(object sender, SensorData data)
        {
            // اینجا داده را به لایه بعدی می‌فرستی (مثلاً APIs در Edge یا queue)
            Console.WriteLine($"Received: {data.DeviceId} {data.SensorType} = {data.Value} at {data.Timestamp}");
            // مثال: ارسال به API
            // await _ingestClient.SendSensorDataAsync(data);
        }

        public async Task StartAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();
            foreach (var adapter in _adapters)
            {
                tasks.Add(adapter.StartAsync(cancellationToken));
            }
            await Task.WhenAll(tasks);
        }

        public void DisposeAll()
        {
            foreach (var adapter in _adapters)
            {
                adapter.Dispose();
            }
        }
    }
}
