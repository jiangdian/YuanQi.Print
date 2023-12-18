using DryIoc;
using Module.BiaoQian.ViewModels;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.IO.Ports;
using YuanQiTool;
using YuanQiUI.Event;

namespace Module.RenGong.ViewModels
{
    public class RenGongViewModel : BindableBase
    {
        #region 属性
        private string _Log = string.Empty;
        public string Log
        {
            get { return _Log; }
            set { SetProperty(ref _Log, value); }
        }
        private string _BarCode = string.Empty;
        public string BarCode
        {
            get { return _BarCode; }
            set 
            { 
                SetProperty(ref _BarCode, value);
                AutoPrint();
            }
        }
        private readonly IEventAggregator eventAggregator;
        private static readonly object oLock = new object();
        private ILogService logService;
        private readonly IContainerExtension container;
        private SerialPort serialPort;
        private BiaoQianViewModel biaoQianViewModel;
        #endregion

        #region 方法
        private void ShowLog(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            lock (oLock)
            {
                if (Log.Length > 2000)
                {
                    Log = "";
                }
                Log += DateTime.Now.ToString("HH:mm:ss- ") + msg + Environment.NewLine;
            }
            logService.Info(msg);
        }
        /// <summary>
        /// 扫码回车自动打印
        /// </summary>
        private void AutoPrint()
        {
            if (BarCode.Length < 10)
            {
                return;
            }
            if (BarCode.EndsWith("\r") || BarCode.EndsWith("\r\n"))
            {
                biaoQianViewModel.PrintSerial(serialPort,BarCode);
                BarCode = "";
              }
    }
        #endregion
        public RenGongViewModel(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.container = container;
            logService = this.container.Resolve<ILogService>();
            serialPort = new SerialPort(SysCfg.Print_COM, 9600, Parity.None, 8, StopBits.One);
            this.eventAggregator.GetEvent<RenGongEvent>().Subscribe(ShowLog);
            biaoQianViewModel= this.container.Resolve<BiaoQianViewModel>();
        }
    }
}
