using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Edge.Services;

namespace ElderlyHealthMonitor.Edge
{
    public class EdgeRunner
    {
        private readonly BleDeviceManager _bleManager = new BleDeviceManager();

        public void Configure()
        {
            // به عنوان مثال شناسه دستگاه یا نام آن را بده
            var xiaomiAdapter = new XiaomiBleAdapter("Mi Band");
            _bleManager.RegisterAdapter(xiaomiAdapter);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await _bleManager.StartAllAsync(cancellationToken);
        }
    }
}
