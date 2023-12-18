using System;
using System.IO.Ports;

namespace YuanQiTool.Scanner.Honeywell
{
    /// <summary>
    /// 扫码枪业务
    /// </summary>
    public class HoneywellSerialScanner
    {
        public HoneywellSerialScanner(string ComName)
        {
            this.ComName = ComName;
        }
        private SerialPort serial;
        public string ComName { get; set; }
        public string BarCodeNo { get; set; } = string.Empty;

        private readonly byte[] bytStart = new byte[] { 0x16, 0x54, 0x0d };//启动读码
        private readonly byte[] bytStop = new byte[] { 0x16, 0x55, 0x0d };//停止读码

        /// <summary>
        /// 连接到扫码器
        /// </summary>
        public bool Connect()
        {
            try
            {
                DisConnect();
                serial = new SerialPort(ComName, 9600, Parity.None, 8, StopBits.One);
                serial.ReadTimeout = 10000;//10s超时
                //serial.DataReceived += SerialPort_DataReceived;
                serial.Open();
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public  void DisConnect()
        {
            try
            {
                serial?.Close();
                serial?.Dispose();
            }
            catch (Exception)
            {
            }
        }
        private  string Receive()
        {
            BarCodeNo = string.Empty;
            try
            {
                serial.NewLine = "\r";
                BarCodeNo = serial.ReadLine();
            }
            catch (Exception)
            {
                BarCodeNo = string.Empty; 
            }
            return BarCodeNo;
        }

        public void ReadBarcode()
        {
            serial.Write(bytStart,0,bytStart.Length);
            BarCodeNo = Receive();
        }
        public void StopRead()
        {
            serial.Write(bytStop, 0, bytStop.Length);
        }
        public void TriggerScan()
        {
            if (Connect())//重新连接，防止历史数据干扰
            {
                ReadBarcode();
                if (string.IsNullOrEmpty(BarCodeNo) || BarCodeNo.Length < 4)//未扫到
                {
                    StopRead();
                    return;
                }
            }
            else
            {
                BarCodeNo=string.Empty;
            }
            DisConnect();
        }
    }
}
