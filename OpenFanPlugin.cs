using FanControl.Plugins;
using System;
using System.Linq;

namespace FanControl.OpenFanPlugin
{
    public class OpenFanPlugin : IPlugin2
    {
        private bool _OpenFanInitialized;
        private OpenFan_Serial _serial;
        private OpenFanManagementControlSensor[] _fanControls;
        private OpenFanManagementFanSensor[] _fanSensors;

        private static readonly int[] _fanIndexes = Enumerable.Range(0, 10).ToArray();

        public string Name => "OpenFAN";
        private readonly IPluginLogger _logger;
        private readonly IPluginDialog _dialog;

        public OpenFanPlugin(IPluginLogger logger, IPluginDialog dialog)
        {
            _logger = logger;
            _dialog = dialog;
        }

        public void Close()
        {
            if (_OpenFanInitialized)
            {
                _OpenFanInitialized = false;
                lock (_serial)
                {
                    _serial.Dispose();
                }

                _fanControls = null;
                _fanSensors = null;
            }
        }

        public void Initialize()
        {
            _OpenFanInitialized = true;
            _logger.Log("OpenFAN plugin loaded.");

            lock(_serial)
            {
                _serial = new OpenFan_Serial();
            }
        }

        public void Load(IPluginSensorsContainer container)
        {
            if (_OpenFanInitialized)
            {
                _fanControls = _fanIndexes
                    .Select(i => new OpenFanManagementControlSensor(i)).ToArray();

                _fanSensors = _fanIndexes
                    .Select(i => new OpenFanManagementFanSensor(i)).ToArray();

                container.ControlSensors.AddRange(_fanControls);
                container.FanSensors.AddRange(_fanSensors);
            }
        }

        public void Update()
        {
            lock (_serial)
            {
                try
                {
                    _serial.Open();

                    var rpms = _serial.ReadRPM();
                    foreach(OpenFanManagementFanSensor fan in _fanSensors)
                    {
                        fan.UpdateFanRPM(rpms);
                    }
                    foreach(OpenFanManagementControlSensor control in _fanControls)
                    {
                        control.SetFanSpeed(_serial);
                    }
                }
                catch (Exception exception)
                {
                    // do something
                }
                finally
                {
                    _serial.Close();
                }
            }
        }
    }
}
