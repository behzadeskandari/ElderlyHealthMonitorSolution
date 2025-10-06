using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Edge.Interfaces;
using ElderlyHealthMonitor.Edge.Models;
using InTheHand.Bluetooth;
using System;
using System.Threading;
using System.Threading.Tasks;
using ElderlyHealthMonitor.Shared.Models;
using InTheHand.Bluetooth;  // مثال پکیج BLE
using System.Linq;

namespace ElderlyHealthMonitor.Edge.Services
{
    public class XiaomiBleAdapter : IDeviceAdapter
    {
        private BluetoothDevice _device;
        private bool _isConnected;

        public event EventHandler<SensorData> OnSensorData;

        public bool IsConnected => _isConnected;

        private readonly string _deviceNameOrId;

        public XiaomiBleAdapter(string deviceNameOrId)
        {
            _deviceNameOrId = deviceNameOrId;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            // اسکن دستگاه‌ها
            var devices = await Bluetooth.ScanForDevicesAsync();  // متد فرضی در InTheHand
            foreach (var dev in devices)
            {
                if (dev.Name != null && dev.Name.Contains(_deviceNameOrId, StringComparison.OrdinalIgnoreCase))
                {
                    _device = dev;
                    break;
                }
            }
            if (_device == null)
                throw new Exception("Xiaomi device not found");

            await _device.PairAsync();
            _isConnected = true;

            // پیدا کردن سرویس Heart Rate و characteristic برای نوتیفای
            var hrService = await _device.Gatt.GetPrimaryServiceAsync(Guid.Parse("0000180d-0000-1000-8000-00805f9b34fb"));
            var hrChar = await hrService.GetCharacteristicAsync(Guid.Parse("00002a37-0000-1000-8000-00805f9b34fb"));

            // فعال کردن نوتیفای
            await hrChar.StartNotificationsAsync();
            hrChar.ValueChanged += HrChar_ValueChanged;

            // شاید بخوای بعضی charهای دیگر (قدم، شتاب) هم فعال کنی
        }

        private void HrChar_ValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            var data = e.Value;  // بایت‌آرایه
            if (data.Length >= 2)
            {
                byte flags = data[0];
                byte hr = data[1];
                var sensor = new SensorData
                {
                    DeviceId = _device.Id.ToString(),
                    Timestamp = DateTimeOffset.UtcNow,
                    SensorType = "heart_rate",
                    Value = hr
                };
                OnSensorData?.Invoke(this, sensor);
            }
        }

        public void Dispose()
        {
            if (_device != null && _isConnected)
            {
                // تمیزکاری
                // _device.DisconnectAsync() یا مشابه
            }
        }
    }
}
