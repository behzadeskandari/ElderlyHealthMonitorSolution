using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Edge.BLE
{
    public class BleScanner
    {
        // This is a skeleton — for production use a library such as Plugin.BLE (Xamarin/Maui) or native bluez code on Linux
        // Responsibilities: scan for devices, parse advertisement or characteristics, create SensorReadingDto batches and send to backend via Http or MQTT


        private readonly IHttpClientFactory _httpFactory;
        private readonly string _backendUrl;


        public BleScanner(IHttpClientFactory httpFactory, string backendUrl)
        {
            _httpFactory = httpFactory;
            _backendUrl = backendUrl;
        }


        public async Task SendReadingBatchAsync(object batch)
        {
            var client = _httpFactory.CreateClient();
            var text = JsonSerializer.Serialize(batch);
            var res = await client.PostAsync(_backendUrl + "/api/readings", new StringContent(text, System.Text.Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
        }


        // TODO: implement native BLE scanning and parsing
    }
}
