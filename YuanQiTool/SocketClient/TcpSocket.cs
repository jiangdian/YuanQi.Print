using System;
using System.Linq;
using System.Net;
using SuperSocket.ClientEngine;

namespace YuanQiTool.SocketClient
{
    public class TcpSocket : ITcpSocket
    {
        AsyncTcpSession client = null;
        //public event Action OnConnecting;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action OnConnectError;
        public event Action<byte[]> OnSocketReceive;
        //public event Action<byte[]> OnSocketSend;

        public TcpSocket()
        {
            client = new AsyncTcpSession();
            client.Connected += OnClientConnected;
            client.Closed += OnClientClosed;
            client.DataReceived += OnReceived;
            client.Error += OnError;
        }

        public bool IsConnected()
        {
            return client.IsConnected;
        }
        public void Connect(string ip, int port)
        {
            try
            {
                client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch (Exception)
            {
            }
          
           
        }

        public void DisConnect()
        {
            client?.Close();
        }

        public void Send(byte[] bytes)
        {
            client.Send(bytes, 0, bytes.Length);
        }


        private void OnReceived(object sender, DataEventArgs e)
        {
          //  var l= e.Length;
            OnSocketReceive?.Invoke(e.Data.Take(e.Length).ToArray());
        }

        private void OnClientConnected(object sender, EventArgs e)
        {
            OnConnected?.Invoke();
        }

        private void OnClientClosed(object sender, EventArgs e)
        {
            OnDisconnected?.Invoke();
        }
        private void OnError(object sender, EventArgs e)
        {
            OnConnectError?.Invoke();
        }

    }
}
