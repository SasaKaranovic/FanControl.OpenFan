using FanControl.Plugins;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FanControl.OpenFanPlugin
{

    public class SerialResponse
    {
        private String _res;
        private int _cmd = 0;
        private List<int> _data = new List<int>();

        public SerialResponse(string data)
        {
            _res = data;
            _parse();
        }

        private void _parse()
        {
            if (_res.Length > 0)
            {
                string[] res = _res.TrimStart('<').Split('|');
                _cmd = Convert.ToInt32(res[0], 16);

                string[] data = res[1].TrimEnd(';').Split(';');
                foreach (string item in data)
                {
                    if (item.Length > 0 && item.Contains(":"))
                    {
                        string[] val = item.Split(':');
                        _data.Add(Convert.ToInt32(val[1], 16));
                    }
                }
            }
        }

        public int getCMD()
        {
            try
            {
                return _cmd;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        public int getData(int pos)
        {
            try
            {
                return _data[pos];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }
    }

    public class OpenFan_Serial
    {
        private SerialPort _serialPort = new SerialPort();
        private bool IsConnectionOpen = false;

        public OpenFan_Serial(string COMPort = "COM15")
        {
            List<string> ports = ComPortNames("2E8A", "000A");

            if (ports.Count == 0)
            {
                throw new Exception("OpenFAN not found");
            }

            _serialPort.PortName = ports[0];
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;
        }

        ~OpenFan_Serial()
        {
            _serialPort.Close();
        }

        public void Close()
        {
            if (IsConnectionOpen)
            {
                _serialPort.Close();
            }
        }

        public int ReadRPM(int index)
        {
            SerialResponse res = SendRequest(">00\r\n");
            return res.getData(index);
        }

        public void SetPercent(int index, int value)
        {
            value = (value * 255 / 100);
            String request = String.Format(">02{0:X2}{1:X2}\r\n", index, value);
            SendRequest(request);
        }


        private SerialResponse SendRequest(string cmd)
        {
            bool _response_found = false;
            String res = "";


            _serialPort.Open();
            IsConnectionOpen = true;
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            _serialPort.Write(cmd);

            // Wait for response
            while (_serialPort.BytesToRead == 0) ;


            while (_response_found == false)
            {
                res = _serialPort.ReadLine();
                if (res.StartsWith("<"))
                {
                    _response_found = true;
                    break;
                }
            }

            _serialPort.Close();

            return new SerialResponse(res.TrimEnd(Environment.NewLine.ToCharArray()));
        }


        static List<string> ComPortNames(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();

            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            string location = (string)rk5.GetValue("LocationInformation");
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            string portName = (string)rk6.GetValue("PortName");
                            if (!String.IsNullOrEmpty(portName) && SerialPort.GetPortNames().Contains(portName))
                                comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

    }
}
