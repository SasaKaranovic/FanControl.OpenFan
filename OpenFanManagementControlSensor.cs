using FanControl.Plugins;

namespace FanControl.OpenFanPlugin
{
    public class OpenFanManagementControlSensor: IPluginControlSensor
    {
        private readonly int _fanIndex;
        private float? _lastSetValue;


        public OpenFanManagementControlSensor(int fanIndex) => _fanIndex = fanIndex;

        public float? Value { get; private set; }

        public string Name => $"OpenFAN Fan #{_fanIndex + 1}";

        public string Origin => $"OpenFAN";

        public string Id => "OpenFan/Control/" + _fanIndex.ToString();

        public void Reset()
        {
            // set back the original control value, is there a command to get the current value we could use
            // to get the original value at the start?
        }

        public void Set(float val)
        {
            _lastSetValue = val;
        }

        public void Update(){ }


        public void SetFanSpeed(OpenFan_Serial serial)
        {
            if (Value != _lastSetValue)
            {
                serial.SetPercent(_fanIndex, (int)_lastSetValue);
                Value = _lastSetValue;
            }
        }
    }
}
