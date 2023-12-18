using S7.Net;
using System;
using System.Collections.Generic;

namespace YuanQiTool.PLC
{
    public class S7Helper : IPLCHelper
    {
        private S7.Net.Plc siemensTcpNet = null;

        public event Action<string> ErrorEvent;

        public void Disconnect()
        {
            siemensTcpNet?.Close();
        }
        public bool PLCConnect(string host, int port = 102)
        {
            try
            {
                siemensTcpNet = new S7.Net.Plc(CpuType.S71200, host, port, 0, 0);
                siemensTcpNet.Open();
                if (siemensTcpNet.IsConnected)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public T Read<T>(string addr)
        {
            try
            {
                return (T)Convert.ChangeType(siemensTcpNet.Read(addr), typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        public bool Write<T>(string addr, T value) 
        {
            try
            {
                siemensTcpNet.Write(addr, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<T> ReadNode<T>(string[] nodes)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
                return siemensTcpNet?.IsConnected??false;
        }

        ~S7Helper()
        {
                siemensTcpNet?.Close();
        }
    }
}
