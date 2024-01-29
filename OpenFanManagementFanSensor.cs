using FanControl.Plugins;
using System;
using System.Net;
using FanControl.OpenFanPlugin;

namespace FanControl.OpenFanPlugin
{
    public class OpenFanManagementFanSensor : IPluginSensor
    {
        private readonly int _fanIndex;

        public OpenFanManagementFanSensor(int fanIndex) => _fanIndex = fanIndex;

        public string Identifier => $"OpenFan/Fan{(int)_fanIndex}";

        public float? Value { get; private set; }

        public string Name => $"OpenFan FAN #{(int)_fanIndex + 1}";

        public string Origin => $"OpenFan";

        public string Id => "Fan_" + _fanIndex.ToString();

        public void Update() => Value = GetFanRPM();

        private int GetFanRPM()
        {
            int rpm = 0;
            OpenFan_Serial OpenFan = new OpenFan_Serial();
            rpm = OpenFan.ReadRPM(_fanIndex);
            OpenFan.Close();

            return rpm;
        }
    }
}
