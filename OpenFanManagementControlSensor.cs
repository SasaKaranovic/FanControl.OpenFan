using FanControl.OpenFanPlugin;
using FanControl.Plugins;
using System;
using System.Net;

namespace FanControl.OpenFanPlugin
{
    public class OpenFanManagementControlSensor: IPluginControlSensor
    {
        private readonly int _fanIndex;
        private float? _val;


        public OpenFanManagementControlSensor(int fanIndex) => _fanIndex = fanIndex;

        public float? Value { get; private set; }

        public string Name => $"OpenFAN Fan #{(int)_fanIndex + 1}";

        public string Origin => $"OpenFAN";

        public string Id => "Control_" + _fanIndex.ToString();

        public void Reset()
        {
            //DellSmbiosBzh.EnableAutomaticFanControl(_fanIndex == BzhFanIndex.Fan1 ? false : true);
        }

        public void Set(float val)
        {
            if (val != _val)
            {
                SetFanSpeed(val);
                _val = val;
            }
        }

        public void Update() => Value = _val;


        private void SetFanSpeed(float speed)
        {
            OpenFan_Serial OpenFan = new OpenFan_Serial();
            OpenFan.SetPercent(_fanIndex, (int)speed);
            OpenFan.Close();
        }
    }
}
