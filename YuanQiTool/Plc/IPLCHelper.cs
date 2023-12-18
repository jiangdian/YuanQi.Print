using System;
using System.Collections.Generic;

namespace YuanQiTool.PLC
{
    public interface IPLCHelper
    {
        event Action<string> ErrorEvent;
        bool PLCConnect(string host, int port);
        T Read<T>(string addr);
        List<T> ReadNode<T>(string[] nodes);
        bool Write<T>(string addr, T value);
        bool IsConnected();
    }
}
