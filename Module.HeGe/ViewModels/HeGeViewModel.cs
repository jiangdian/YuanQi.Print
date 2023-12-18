using DryIoc;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuanQiTool;
using YuanQiTool.Print;
using YuanQiUI.Event;
using YuanQiUI.ViewModels;

namespace Module.HeGe.ViewModels
{
    public class HeGeViewModel :BaseViewModel
    {
        
        #region 方法
        #region 加载配置文件
        public override void LoadCfg()
        {
            string fileName = GetCfgFile("合格证");
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
                string fileName = SaveCfgFile("合格证");
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
                SysCfg.SetConfiguration("HOffsetX", OffsetX);
                SysCfg.SetConfiguration("HOffsetY", OffsetY);
                SysCfg.SetConfiguration("HDPI", DPI);
                SysCfg.SetConfiguration("HRotate180", Rotate180);
                SysCfg.SetConfiguration("HLableTop", LableTop);
                SysCfg.SetConfiguration("HTMBody", BarcodeCfg.PrintBody);
                MessageBox.Show("保存完毕！");
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("保存配置失败！", ex);
                MessageBox.Show("保存失败！");
            }
        }
        #endregion
        #region 生成ZPL
        public override void ManualGenerateZPL()
        {
            GenerateZPL();
        }
        public override void GenerateZPL()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("^XA");//开始
            if (this.Rotate180)
            {
                builder.AppendLine("^POI");
            }
            else
            {
                builder.AppendLine("^PON");
            }
            builder.AppendLine($"^LT{LableTop}^LH{OffsetX},{OffsetY}");//标签整体偏移
            #region 型号
            int x_lefttop = PrintHelper.GetDot(LeftTopCfg.X, DPI);
            int y_lefttop = PrintHelper.GetDot(LeftTopCfg.Y, DPI);
            int w_lefttop = PrintHelper.GetDot(LeftTopCfg.Width, DPI);
            int h_lefttop = PrintHelper.GetDot(LeftTopCfg.Height, DPI);
            var lt_data = PrintHelper.getFontText(LeftTopCfg.PrintHead + LeftTopCfg.PrintBody,
                LeftTopCfg.FontName, Orientation.O_270,
                h_lefttop, w_lefttop, LeftTopCfg.IsBold, false);
            builder.Append(lt_data.GetDateString("LT.GRF"));//下载图像内容
            builder.Append($"^FO{x_lefttop},{y_lefttop},0");//位置
            builder.AppendLine($"^XGLT.GRF,1,1^FS");//打印图像内容 1表示不缩放
            #endregion
            #region 检验
            int x_righttop = PrintHelper.GetDot(RightTopCfg.X, DPI);
            int y_righttop = PrintHelper.GetDot(RightTopCfg.Y, DPI);
            int w_righttop = PrintHelper.GetDot(RightTopCfg.Width, DPI);
            int h_righttop = PrintHelper.GetDot(RightTopCfg.Height, DPI);

            var rt_data = PrintHelper.getFontText(RightTopCfg.PrintHead + RightTopCfg.PrintBody,
               RightTopCfg.FontName, Orientation.O_270,
               h_righttop, w_righttop, RightTopCfg.IsBold, false);
            builder.Append(rt_data.GetDateString("RT.GRF"));//下载图像内容
            builder.Append($"^FO{x_righttop},{y_righttop},0");//位置
            builder.Append($"^XGRT.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");

            #endregion
            #region 日期
            int x_leftbottom = PrintHelper.GetDot(LeftBottomCfg.X, DPI);
            int y_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Y, DPI);
            int w_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Width, DPI);
            int h_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Height, DPI);

            var lb_data = PrintHelper.getFontText(LeftBottomCfg.PrintHead + LeftBottomCfg.PrintBody,
              LeftBottomCfg.FontName, Orientation.O_270,
              h_leftbottom, w_leftbottom, LeftBottomCfg.IsBold, false);
            builder.Append(lb_data.GetDateString("LB.GRF"));//下载图像内容
            builder.Append($"^FO{x_leftbottom},{y_leftbottom},0");//位置
            builder.AppendLine($"^XGLB.GRF,1,1^FS");//打印图像内容 1表示不缩放
            #endregion

            #region PA
            int x_rightbottom = PrintHelper.GetDot(RightBottomCfg.X, DPI);
            int y_rightbottom = PrintHelper.GetDot(RightBottomCfg.Y, DPI);
            int w_rightbottom = PrintHelper.GetDot(RightBottomCfg.Width, DPI);
            int h_rightbottom = PrintHelper.GetDot(RightBottomCfg.Height, DPI);
            builder.Append($"^FO{x_rightbottom},{y_rightbottom},0\r\n");//位置
            var rb_data = PrintHelper.getFontText(RightBottomCfg.PrintHead + RightBottomCfg.PrintBody,
                RightBottomCfg.FontName, Orientation.O_270,
                h_rightbottom, w_rightbottom, RightBottomCfg.IsBold, false);//内容
            builder.Append(rb_data.GetDateString("LB.GRF"));//下载图像内容
            builder.Append($"^FO{x_rightbottom},{y_rightbottom},0");//位置
            builder.AppendLine($"^XGLB.GRF,1,1^FS");//打印图像内容 1表示不缩放
            #endregion
            #region 条码
            int x_barcode = PrintHelper.GetDot(BarcodeCfg.X, DPI);
            int y_barcode = PrintHelper.GetDot(BarcodeCfg.Y, DPI);
            int w_barcode = PrintHelper.GetDot(BarcodeCfg.Width, DPI);
            int h_barcode = PrintHelper.GetDot(BarcodeCfg.Height, DPI);
            builder.Append($"^FO{x_barcode},{y_barcode},0\r\n");//位置
            builder.Append($"^BY{w_barcode},3.0,{h_barcode}");//尺寸
            //BarcodeCfg.PrintBody = SysCfg.HTMBody;
            string txt_barcode = $"^BCB,,N,N,N,N^FD>:{BarcodeCfg.PrintHead}{BarcodeCfg.PrintBody.PadLeft(5, '0')}^FS";
            builder.AppendLine(txt_barcode);
            builder.Append("^XZ");
            #endregion
            this.ZPL = builder.ToString();
        }
        public override bool CreatZPL(string barcode)
        {
            try
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish($"生成合格证{BarcodeCfg.PrintHead}{BarcodeCfg.PrintBody}ZPL...");
                GenerateZPL();
            }
            catch (Exception)
            {
                string errMsg = "生成合格证ZPL失败！";
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
                tcp.Connect(SysCfg.PRINTERH_IP, SysCfg.PRINTERH_PORT);
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
                this.eventAggregator.GetEvent<MessageEvent>().Publish("发送至合格证打印机...");
                if (!ConnectPrinter())
                {
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("连接合格证打印机失败!");
                    return false;
                }
                byte[] data = Encoding.ASCII.GetBytes(ZPL);
                print.Send(data);
            }
            catch (Exception)
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish("发送至合格证打印机失败!");
                return false;
            }
            return true;
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
                tcp.Connect(SysCfg.PRINTERH_IP, SysCfg.PRINTERH_PORT);
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
            print_ip = SysCfg.PRINTERH_IP;
            print_port = SysCfg.PRINTERH_PORT;
            print_info = "合格证";
        }
        #endregion

        public HeGeViewModel(IEventAggregator eventAggregator):base(eventAggregator)
        {
            base.PropertyChanged-=this.OnPropertyChanged;
            DPI = SysCfg.HDPI;
            Rotate180 = SysCfg.HRotate180;
            OffsetX = SysCfg.HOffsetX;
            OffsetY = SysCfg.HOffsetY;
            LableTop = SysCfg.HLableTop;
            LeftTopCfg = new PrintCfg(8, 6, 1.2, 2.4, true, false, "黑体", "DDZY3699-Z");
            RightTopCfg = new PrintCfg(12, 10, 1.2, 2.4, true, false, "黑体", "QC02");
            LeftBottomCfg = new PrintCfg(19, 5, 1.2, 2.4, true, false, "黑体", "2023-01-04");           
            RightBottomCfg = new PrintCfg(24, 5, 1.2, 2.4, true, false, "黑体", "2022E864-37");
            BarcodeCfg = new PrintCfg(27, 4, 0.2, 3, false, false, "", "00001", "D314");
            BarcodeCfg.PrintBody=SysCfg.HTMBody;
            BarcodeCfg.PropertyChanged += BarcodeCfg_PropertyChanged;
            base.PropertyChanged += this.OnPropertyChanged;
        }
        private  void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "ZPL" || args.PropertyName == "BarcodeCfg")
            {
            }
            else
            {
                var value = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
                SysCfg.SetConfiguration($"H{args.PropertyName}", value);
            }
        }
        private void BarcodeCfg_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName== "PrintBody")
            {
                SysCfg.SetConfiguration("HTMBody", BarcodeCfg.PrintBody);
            }
        }
    }
}
