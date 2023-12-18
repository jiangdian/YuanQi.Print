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
using System.Threading;
using System.Threading.Tasks;
using YuanQiTool;
using YuanQiTool.Print;
using YuanQiTool.SocketClient;
using YuanQiUI.Event;
using YuanQiUI.ViewModels;

namespace YuanQiUI.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        #region 属性
        private PrintCfg _lt;
        private PrintCfg _rt;
        private PrintCfg _lb;
        private PrintCfg _rb;
        public PrintCfg _bar;
        private string _ZPL;
        private int _DPI = 300;
        private bool _Rotate180 = false;
        private int _StartCut = 0;
        private int _EndCut = 0;
        private string _CodeBefore;
        private int _LableTop;
        private int _OffsetX, _OffsetY;
        public PrintCfg LeftTopCfg { get => _lt; set => SetProperty(ref _lt, value); }
        public PrintCfg LeftBottomCfg { get => _lb; set => SetProperty(ref _lb, value); }
        public PrintCfg RightTopCfg { get => _rt; set => SetProperty(ref _rt, value); }
        public PrintCfg RightBottomCfg { get => _rb; set => SetProperty(ref _rb, value); }
        public PrintCfg BarcodeCfg { get => _bar; set => SetProperty(ref _bar, value); }
        public string ZPL { get => _ZPL; set => SetProperty(ref _ZPL, value); }
        public int DPI { get => _DPI; set => SetProperty(ref _DPI, value); }
        public int OffsetX { get => _OffsetX; set => SetProperty(ref _OffsetX, value); }
        public int OffsetY { get => _OffsetY; set => SetProperty(ref _OffsetY, value); }
        public bool Rotate180 { get => _Rotate180; set => SetProperty(ref _Rotate180, value); }
        public int StartCut { get => _StartCut; set => SetProperty(ref _StartCut, value); }
        public int EndCut { get => _EndCut; set => SetProperty(ref _EndCut, value); }
        public string CodeBefore { get => _CodeBefore; set => SetProperty(ref _CodeBefore, value); }
        public int LableTop { get => _LableTop; set => SetProperty(ref _LableTop, value); }
        public readonly IEventAggregator eventAggregator;
        public ITcpSocket print;
        public string print_ip;
        public int print_port;
        public string print_info;
        #endregion
        #region Command
        /// <summary>
        /// Load 加载配置项
        /// </summary>
        private DelegateCommand _LoadCfgCmd;
        public DelegateCommand LoadCfgCmd =>
            _LoadCfgCmd ?? (_LoadCfgCmd = new DelegateCommand(LoadCfg));
        /// <summary>
        /// Save 保存配置项
        /// </summary>
        private DelegateCommand _SaveCfgCmd;
        public DelegateCommand SaveCfgCmd =>
            _SaveCfgCmd ?? (_SaveCfgCmd = new DelegateCommand(SaveCfg));
        /// <summary>
        /// ZPL 生成ZPL
        /// </summary>
        private DelegateCommand _ManualGenerateZPLCmd;
        public DelegateCommand ManualGenerateZPLCmd =>
            _ManualGenerateZPLCmd ?? (_ManualGenerateZPLCmd = new DelegateCommand(ManualGenerateZPL));
        /// <summary>
        /// Print 打印
        /// </summary>
        private DelegateCommand _PrintCmd;
        public DelegateCommand PrintCmd =>
            _PrintCmd ?? (_PrintCmd = new DelegateCommand(Print));
        /// <summary>
        /// Print 打印
        /// </summary>
        private DelegateCommand _ClearCmd;
        public DelegateCommand ClearCmd =>
            _ClearCmd ?? (_ClearCmd = new DelegateCommand(Clear));
        #endregion
        #region 方法
        #region 加载配置文件
        public virtual  void LoadCfg()
        {
        }
        public  string GetCfgFile(string sTitle)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + sTitle;
            fileDialog.Filter = "*.ini|*.ini";
            fileDialog.Multiselect = false;
            if (!fileDialog.ShowDialog().GetValueOrDefault())
            {
                return null;
            }
            return fileDialog.FileName;
        }
        public void LoadCfg(string fileName)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(fileName))
                    {
                        this.eventAggregator.GetEvent<MessageEvent>().Publish($"{fileName}不存在!");
                        return;
                    }
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("开始加载配置..." + fileName);
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    byte[] buffer = new byte[200 * 1024];
                    int len = fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    fs.Dispose();
                    byte[] data = new byte[len];
                    Array.Copy(buffer, data, len);
                    string strCfgs = Encoding.UTF8.GetString(data);
                    BindingList<PrintCfg> cfgs = JsonConvert.DeserializeObject<BindingList<PrintCfg>>(strCfgs);
                    if (cfgs == null || cfgs.Count < 5)
                    {
                        this.eventAggregator.GetEvent<MessageEvent>().Publish("配置文件格式错误或配置数量少于5个!");
                        return;
                    }
                    LeftTopCfg = cfgs[0];
                    RightTopCfg = cfgs[1];
                    LeftBottomCfg = cfgs[2];
                    RightBottomCfg = cfgs[3];
                    BarcodeCfg = cfgs[4];
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("配置加载完毕!");
                }
                catch (Exception ex)
                {
                    LogService.Instance.Error("加载配置失败！", ex);
                    this.eventAggregator.GetEvent<MessageEvent>().Publish("加载配置失败！");
                }
            });
        }
        #endregion
        #region 保存配置文件
        public virtual void SaveCfg()
        {           
        }
        public string SaveCfgFile(string sTitle)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "*.ini|*.ini";     //设置保存类型
            save.Title = sTitle;   //对话框标题
            save.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + sTitle;    //设置初始打开目录

            if (save.ShowDialog() == false)
            {
                return null;
            }
            return save.FileName;
        }
        #endregion
        #region 生成ZPL
        public virtual void ManualGenerateZPL()
        {
        }
        public virtual void AdjustPrintBarcode(string barcode)
        {
        }
        public virtual void GenerateZPL()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("^XA");//开始
            if (this.Rotate180)
            {
                builder.Append("^POI\r\n");
            }
            else
            {
                builder.Append("^PON\r\n");
            }
            builder.Append($"^LT{LableTop}^LH{OffsetX},{OffsetY}\r\n");//标签整体偏移
            #region 左上 - 汉字
            int x_lefttop = PrintHelper.GetDot(LeftTopCfg.X, DPI);
            int y_lefttop = PrintHelper.GetDot(LeftTopCfg.Y, DPI);
            int w_lefttop = PrintHelper.GetDot(LeftTopCfg.Width, DPI);
            int h_lefttop = PrintHelper.GetDot(LeftTopCfg.Height, DPI);
            var lt_data = PrintHelper.getFontText(LeftTopCfg.PrintHead + LeftTopCfg.PrintBody,
                LeftTopCfg.FontName, Orientation.Zero,
                h_lefttop, w_lefttop, LeftTopCfg.IsBold, false);
            builder.Append(lt_data.GetDateString("LT.GRF"));//下载图像内容
            builder.Append($"^FO{x_lefttop},{y_lefttop},0");//位置
            builder.Append($"^XGLT.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");
            #endregion
            #region 右上 - 英文+.+数字
            int x_righttop = PrintHelper.GetDot(RightTopCfg.X, DPI);
            int y_righttop = PrintHelper.GetDot(RightTopCfg.Y, DPI);
            int w_righttop = PrintHelper.GetDot(RightTopCfg.Width, DPI);
            int h_righttop = PrintHelper.GetDot(RightTopCfg.Height, DPI);

            var rt_data = PrintHelper.getFontText(RightTopCfg.PrintHead + RightTopCfg.PrintBody,
               RightTopCfg.FontName, Orientation.Zero,
               h_righttop, w_righttop, RightTopCfg.IsBold, false);
            builder.Append(rt_data.GetDateString("RT.GRF"));//下载图像内容
            builder.Append($"^FO{x_righttop},{y_righttop},0");//位置
            builder.Append($"^XGRT.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");

            //纯英文方法
            //builder.Append($"^FO{x_righttop},{y_righttop},0");//位置
            //builder.Append($"^A0N,{h_righttop},{w_righttop}");//大小
            //string txt_righttop = RightTopCfg.PrintHead + RightTopCfg.PrintBody;
            //builder.Append($"^FD{txt_righttop}^FS"); //打印内容
            //builder.Append("\r\n");

            #endregion
            #region 左下 - 数字
            int x_leftbottom = PrintHelper.GetDot(LeftBottomCfg.X, DPI);
            int y_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Y, DPI);
            int w_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Width, DPI);
            int h_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Height, DPI);

            var lb_data = PrintHelper.getFontText(LeftBottomCfg.PrintHead + LeftBottomCfg.PrintBody,
              LeftBottomCfg.FontName, Orientation.Zero,
              h_leftbottom, w_leftbottom, LeftBottomCfg.IsBold, false);
            builder.Append(lb_data.GetDateString("LB.GRF"));//下载图像内容
            builder.Append($"^FO{x_leftbottom},{y_leftbottom},0");//位置
            builder.Append($"^XGLB.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");

            //builder.Append($"^FO{x_leftbottom},{y_leftbottom},0");//位置
            //builder.Append($"^A0N,{h_leftbottom},{w_leftbottom}");//大小
            //string txt_leftbottom = LeftBottomCfg.PrintHead + LeftBottomCfg.PrintBody;//内容
            //builder.Append($"^FD{txt_leftbottom}^FS"); //打印内容
            //builder.Append("\r\n");
            #endregion

            #region 右下 - 汉字
            int x_rightbottom = PrintHelper.GetDot(RightBottomCfg.X, DPI);
            int y_rightbottom = PrintHelper.GetDot(RightBottomCfg.Y, DPI);
            int w_rightbottom = PrintHelper.GetDot(RightBottomCfg.Width, DPI);
            int h_rightbottom = PrintHelper.GetDot(RightBottomCfg.Height, DPI);
            builder.Append($"^FO{x_rightbottom},{y_rightbottom},0\r\n");//位置
            var rb_data = PrintHelper.getFontText(RightBottomCfg.PrintHead + RightBottomCfg.PrintBody,
                RightBottomCfg.FontName, Orientation.Zero,
                h_rightbottom, w_rightbottom, RightBottomCfg.IsBold, false);//内容
            builder.Append(rb_data.GetDateString("LB.GRF"));//下载图像内容
            builder.Append($"^FO{x_rightbottom},{y_rightbottom},0");//位置
            builder.Append($"^XGLB.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");
            #endregion
            #region 条码
            int x_barcode = PrintHelper.GetDot(BarcodeCfg.X, DPI);
            int y_barcode = PrintHelper.GetDot(BarcodeCfg.Y, DPI);
            int w_barcode = PrintHelper.GetDot(BarcodeCfg.Width, DPI);
            int h_barcode = PrintHelper.GetDot(BarcodeCfg.Height, DPI);
            builder.Append($"^FO{x_barcode},{y_barcode},0\r\n");//位置
            builder.Append($"^BY{w_barcode},3.0,{h_barcode}");//尺寸
            string txt_barcode = $"^BCN,,N,N,N,N^FD>;{BarcodeCfg.PrintBody}^FS";
            builder.Append(txt_barcode);
            builder.Append("\r\n");
            builder.Append("^XZ");
            #endregion
            this.ZPL = builder.ToString();
        }
        public virtual bool CreatZPL(string barcode)
        { return false; }
        #endregion
        #region 打印
        public virtual void Print()
        {
        }
        public virtual bool SendPrint()
        {
            return false;
        }
        #endregion
        #region 清除缓存
        public virtual void Clear()
        {  
        }
        #endregion
        #region 连接打印机
        public  bool ConnectPrinter()
        {
            print?.DisConnect();
            print = new TcpSocket();
            GetPrintInfo();
            this.eventAggregator.GetEvent<MessageEvent>().Publish($"连接{print_info}打印机...");
            print.Connect(print_ip,print_port);
            Thread.Sleep(500);
            if (!print.IsConnected())
            {
                this.eventAggregator.GetEvent<MessageEvent>().Publish($"连接{print_info}打印机失败!");
                return false;
            }
            this.eventAggregator.GetEvent<MessageEvent>().Publish($"连接{print_info}打印机成功!");
            return true;
        }
        public virtual void GetPrintInfo()
        {
        }
        #endregion
        #endregion
        public BaseViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }
    }
}
