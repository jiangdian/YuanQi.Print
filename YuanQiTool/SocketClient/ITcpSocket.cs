using System;

namespace YuanQiTool.SocketClient
{
    public interface ITcpSocket
    {
        // event Action OnConnecting;
        event Action OnConnected;
        event Action OnDisconnected;
        event Action<byte[]> OnSocketReceive;
        // event Action<byte[]> OnSocketSend;

        bool IsConnected();
        void Connect(string ip, int port);
        void DisConnect();
        void Send(byte[] bytes);

    }
}
