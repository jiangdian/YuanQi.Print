using Newtonsoft.Json;
using Prism.Events;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using YuanQiTool;
using YuanQiUI.Event;
using YuanQiUI.ViewModels;

namespace Module.BiaoQian.ViewModels
{
    public class BiaoQianViewModel :BaseViewModel
    {
        #region 方法
        #region 加载配置文件
        public override void LoadCfg()
        {
            string fileName = GetCfgFile("二次标签");
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            LoadCfg(fileName);
        }
        #endregion
        #region 保存配置文件
        public override void SaveCfg()
        {
            try
            {
                string fileName = SaveCfgFile("二次标签");
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }
                BindingList<PrintCfg> cfgs = new BindingList<PrintCfg>
                {
                    LeftTopCfg,
                    RightTopCfg,
                    LeftBottomCfg,
                    RightBottomCfg,
                    BarcodeCfg
                };
                string strJson = JsonConvert.SerializeObject(cfgs);
                FileStream fs = new FileStream(fileName, FileMode.Create);
                byte[] buffer = Encoding.UTF8.GetBytes(strJson);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                fs.Dispose();
                SysCfg.SetConfiguration("OffsetX", OffsetX);
                SysCfg.SetConfiguration("OffsetY", OffsetY);
                SysCfg.SetConfiguration("DPI", DPI);
                SysCfg.SetConfiguration("Rotate180", Rotate180);
                SysCfg.SetConfiguration("StartCut", StartCut);
                SysCfg.SetConfiguration("EndCut", EndCut);
                SysCfg.SetConfiguration("CodeBefore", CodeBefore);
                SysCfg.SetConfiguration("LableTop", LableTop);
                MessageBox.Show("保存完毕！");
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("保存配置失败！", ex);
                MessageBox.Show("保存失败！");
            }
        }
        #endregion
        #region 生产ZPL
        public override void ManualGenerateZPL()
        {
            AdjustPrintBarcode(BarcodeCfg.PrintBody);
            GenerateZPL();
        }
        public override void AdjustPrintBarcode(string barcode)
        {
            BarcodeCfg.PrintBody = barcode;
            LeftBottomCfg.PrintBody = BarcodeCfg.PrintBody;
            RightTopCfg.PrintBody = "";
            //截取条码，前补字符
            int len = this.EndCut - this.StartCut + 1;
            if (len < 0)
            {
                len = 0;
            }
            int startIndex = this.StartCut - 1;
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            string strCut;
            try
            {
                strCut = BarcodeCfg.PrintBody.Substring(startIndex, len);
            }
            catch
            {
                string errMsg = $"条码{BarcodeCfg.PrintBody}截取失败，条码长度{BarcodeCfg.PrintBody.Length}；NO截取参数：{StartCut}~{EndCut}位，长度{len}。";
                BarcodeCfg.PrintBody = "";
                this.eventAggregator.GetEvent<MessageEvent>().Publish(errMsg);
                return;
            }
            strCut = CodeBefore + strCut;
            RightTopCfg.PrintBody = strCut;
        }
        public override bool CreatZPL(string barcode)
        {
            try
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish($"生成{barcode}二次标签ZPL...");
                AdjustPrintBarcode(barcode);
                GenerateZPL();
            }
            catch (Exception)
            {
                string errMsg = "生成二次标签ZPL失败！";
                this.eventAggregator.GetEvent<MessageEvent>().Publish(errMsg);
                return false;
            }
            return true;
        }
        #endregion
        #region 打印
        public override void Print()
        {
            try
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("准备发送命令至打印机...");
                var tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
                tcp.SendTimeout = 3000;
                tcp.ReceiveTimeout = 3000;
                tcp.Connect(SysCfg.PRINTER_IP, SysCfg.PRINTER_PORT);
                if (!tcp.Connected)
                {
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("连接打印机失败!");
                    return;
                }
                this.eventAggregator.GetEvent<MessageEvent>().Publish("连接打印机成功!");
                int len = tcp.Send(Encoding.ASCII.GetBytes(ZPL));
                this.eventAggregator.GetEvent<MessageEvent>().Publish("发送成功!");
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("打印失败！", ex);
                this.eventAggregator.GetEvent<MessageEvent>().Publish("打印失败!");
                return;
            }
        }
        public override bool SendPrint()
        {
            try
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("发送至二次标签打印机...");
                if (!ConnectPrinter())
                {
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("连接二次标签打印机失败!");
                    return false;
                }
                byte[] data = Encoding.ASCII.GetBytes(ZPL);
                print.Send(data);
            }
            catch (Exception)
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("发送至二次标签打印机失败!");
                return false;
            }
            return true;
        }
        public  void PrintSerial(SerialPort serialPort,string barcode)
        {
            barcode = barcode.Trim('\r', '\n');
            this.eventAggregator.GetEvent<RenGongEvent>().Publish($"当前打印条码{barcode}");
            AdjustPrintBarcode(barcode);
            GenerateZPL();
            try
            {
                // 打开串口
                serialPort.Open();
                // 发送指令到打印机
                serialPort.Write(ZPL);
                // 关闭串口
                serialPort.Close();
            }
            catch (Exception ex)
            {
                this.eventAggregator.GetEvent<RenGongEvent>().Publish("串口通信出现异常：" + ex.Message);
            }
            finally
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Dispose();
            }
        }
        #endregion
        #region 清除缓存
        public override void Clear()
        {
            try
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("准备发送命令至打印机...");
                var tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
                tcp.SendTimeout = 3000;
                tcp.ReceiveTimeout = 3000;
                tcp.Connect(SysCfg.PRINTER_IP, SysCfg.PRINTER_PORT);
                if (!tcp.Connected)
                {
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("连接打印机失败!");
                    return;
                }
                this.eventAggregator.GetEvent<MessageEvent>().Publish("连接打印机成功!");
                int len = tcp.Send(Encoding.ASCII.GetBytes("~JA"));
                this.eventAggregator.GetEvent<MessageEvent>().Publish("发送成功!");
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("清除缓存失败！", ex);
                this.eventAggregator.GetEvent<MessageEvent>().Publish("清除缓存失败!");
                return;
            }          
        }
        #endregion
        public override void GetPrintInfo()
        {
            print_ip = SysCfg.PRINTER_IP;
            print_port = SysCfg.PRINTER_PORT;
            print_info = "二次标签";
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "ZPL" || args.PropertyName == "BarcodeCfg")
            {
            }
            else
            {
                var value = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
                SysCfg.SetConfiguration(args.PropertyName, value);
            }
        }
        #endregion
        public BiaoQianViewModel(IEventAggregator eventAggregator):base(eventAggregator)
        {
            base.PropertyChanged -= this.OnPropertyChanged;
            DPI = SysCfg.DPI;
            Rotate180 = SysCfg.Rotate180;
            StartCut = SysCfg.StartCut;
            EndCut = SysCfg.EndCut;
            CodeBefore = SysCfg.CodeBefore;
            OffsetX = SysCfg.OffsetX;
            OffsetY = SysCfg.OffsetY;
            LableTop = SysCfg.LableTop;
            LeftTopCfg = new PrintCfg(2, 1, 1.4, 3.5, true, false, "黑体", "国家电网");
            LeftBottomCfg = new PrintCfg(2, 12.7, 1.5, 3.8, true, false, "黑体", "1540001011500333473753");
            RightTopCfg = new PrintCfg(30, 1, 1.4, 38, true, false, "黑体", "01150033347375", "NO.");
            RightBottomCfg = new PrintCfg(45.7, 12.6, 1.5, 3.7, true, false, "黑体", "2019年");
            BarcodeCfg = new PrintCfg(2, 4.6, 0.3, 8, false, false, "", "1540001011500333473753", "");
            base.PropertyChanged += this.OnPropertyChanged;
        }
        
    }
}
