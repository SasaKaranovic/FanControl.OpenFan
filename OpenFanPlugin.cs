using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FanControl.OpenFanPlugin
{
    public class OpenFanPlugin : IPlugin2, IDisposable
    {
        private bool _OpenFanInitialized;
        private Boolean m_DisposedValue;

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
            }
        }

        public void Initialize()
        {
            _OpenFanInitialized = true;
            _logger.Log("OpenFAN plugin loaded.");
        }

        public void Load(IPluginSensorsContainer _container)
        {
            if (_OpenFanInitialized)
            {
                IEnumerable<OpenFanManagementControlSensor> fanControls = new[] {
                            0, 1, 2, 3, 4, 5, 6, 7, 8, 9
                        }.Select(i => new OpenFanManagementControlSensor(i)).ToArray();

                IEnumerable<OpenFanManagementFanSensor> fanSensors = new[] {
                            0, 1, 2, 3, 4, 5, 6, 7, 8, 9
                        }.Select(i => new OpenFanManagementFanSensor(i)).ToArray();

                _container.ControlSensors.AddRange(fanControls);
                _container.FanSensors.AddRange(fanSensors);
            }
        }

        public void Update()
        {
           
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
                  // TODO: dispose managed state (managed objects)
                }

              // TODO: free unmanaged resources (unmanaged objects) and override finalizer
              // TODO: set large fields to null
              Close();
              m_DisposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
         ~OpenFanPlugin()
         {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
         }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
