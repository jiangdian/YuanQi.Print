using System;
using System.Text;
using YuanQiTool.SocketClient;

namespace YuanQiTool.Scanner.Honeywell
{
    /// <summary>
    /// 扫码枪业务
    /// </summary>
    public class HoneywellTCPScanner
    {
        private ITcpSocket m_SocketClient;
        public string BarCodeNo { get; set; } = "";

        private readonly byte[] bytStart = new byte[] { 0x16, 0x54, 0x0d };//启动读码
        private readonly byte[] bytStop = new byte[] { 0x16, 0x55, 0x0d };//停止读码

        /// <summary>
        /// 连接到扫码器
        /// </summary>
        public void Connect(string ip, int port)
        {
            m_SocketClient?.DisConnect();
            m_SocketClient = new TcpSocket();
            m_SocketClient.OnSocketReceive += OnReceive;//接收消息
            m_SocketClient.OnConnected += OnConnected;
            m_SocketClient.OnDisconnected += OnDisconnected;//断开连接
            m_SocketClient.Connect(ip, port);//创建连接
        }

        public bool IsConnect()
        {
            return m_SocketClient.IsConnected();
        }
        /// <summary>
        /// 连接成功触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnected()
        {

        }
        /// <summary>
        /// 当断开连接触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDisconnected()
        {

        }
        void OnConnectedError(object sender, EventArgs e)
        {
            m_SocketClient.DisConnect();
        }
        void OnReceive(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return;
            }
            try
            {
                BarCodeNo = Encoding.ASCII.GetString(buffer).Trim();
            }
            catch (Exception)
            {
                BarCodeNo = "";
            }
        }
        public void ReadBarcode()
        {
            m_SocketClient.Send(bytStart);
        }
        public void StopRead()
        {
            m_SocketClient.Send(bytStop);
        }
        public void DisConnect()
        {
            m_SocketClient?.DisConnect();
        }
    }
}
