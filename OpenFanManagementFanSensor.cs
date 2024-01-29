using FanControl.Plugins;
using System.Collections.Generic;

namespace FanControl.OpenFanPlugin
{
    public class OpenFanManagementFanSensor : IPluginSensor
    {
        private readonly int _fanIndex;

        public OpenFanManagementFanSensor(int fanIndex)
        {
            _fanIndex = fanIndex;
        }

        public string Identifier => $"OpenFan/Fan/{_fanIndex}";

        public float? Value { get; private set; }

        public string Name => $"OpenFan FAN #{_fanIndex + 1}";

        public string Origin => $"OpenFan";

        public string Id => "Fan_" + _fanIndex.ToString();

        public void Update() { }

        public void UpdateFanRPM(SerialResponse response)
        {
            int rpm = response.Data[_fanIndex];
            Value = rpm;
        }
    }
}
