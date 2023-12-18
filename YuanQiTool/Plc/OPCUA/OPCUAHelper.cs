using Opc.Ua;
using OpcUaHelper;
using System;
using System.Collections.Generic;

namespace YuanQiTool.PLC.OPCUA
{
    public class OPCUAHelper : IPLCHelper
    {
        private OpcUaClient client;
        public event Action<string> ErrorEvent;
        public OPCUAHelper()
        {
            client= new OpcUaClient();
            client.UserIdentity = new UserIdentity(new AnonymousIdentityToken());
        }
        public bool IsConnected()
        {
            return client.Connected;
        }
        public void Disconnect()
        {
            client?.Disconnect();
        }
        public bool PLCConnect(string host, int port)
        {
            try
            {
                client?.Disconnect();
                client.ConnectServer($"opc.tcp://{host}:{port}").Wait();
                ErrorEvent?.Invoke("连接OPC成功");
                return client.Connected; 
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke("连接OPC异常【" + ex.Message + "】");
                return false;
            }
        }
        public T Read<T>(string addr)
        {
            try
            {
                var value = client.ReadNode<T>(addr);
                return value;
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke($"读取OPC【{addr}】异常【" + ex.Message + "】");
                return default(T);
            }            
        }
        public List<T> ReadNode<T>(string[] nodes)
        {
            try
            {
                var value = client.ReadNodes<T>(nodes);
                return value;
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke("读取OPC【】异常【" + ex.Message + "】");
                return null;
            }
        }
        public bool Write<T>(string addr, T value)
        {
            try
            {
                return client.WriteNode<T>(addr, value);
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke("写OPC异常【" + ex.Message + "】");
                return false;
            }           
        }
    }
}
