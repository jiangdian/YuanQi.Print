using DryIoc;
using Module.Home.Bll;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using YuanQiTool;
using YuanQiUI.Event;

namespace Module.Home.ViewModels
{
    public class HomeViewModel : BindableBase
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
            set { SetProperty(ref _BarCode, value); }
        }
        private readonly IEventAggregator eventAggregator;
        private static readonly object oLock = new object();
        private ILogService logService;
        private readonly IContainerExtension container;
        private HomeBll homeBll;

        #endregion
        #region Command
        /// <summary>
        /// 自动控制
        /// </summary>
        private DelegateCommand _AutoCtlCmd;
        public DelegateCommand AutoCtlCmd =>
            _AutoCtlCmd ?? (_AutoCtlCmd = new DelegateCommand(AutoCtl));
        private  void AutoCtl()
        {            
            if (!SysCfg.BiaoQian && !SysCfg.HeGe)//选择打印类型
            {
                ShowLog("请先前往设置页面选择打印内容!");
                return;
            }
            Stop();
            Task.Run(() => {
                if (!homeBll.Init()) return;
                homeBll.ListenPLC();
            });           
        }
        /// <summary>
        /// 自动控制
        /// </summary>
        private DelegateCommand _StopCmd;
        public DelegateCommand StopCmd =>
            _StopCmd ?? (_StopCmd = new DelegateCommand(Stop));
        private void Stop()
        {
            homeBll.Stop();
        }
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
        private void ShowBarCode(string barcode)
        {
            BarCode = barcode;
        }
        #endregion
        public HomeViewModel(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.container = container;
            this.eventAggregator.GetEvent<MessageEvent>().Subscribe(ShowLog);
            this.eventAggregator.GetEvent<BarCodeEvent>().Subscribe(ShowBarCode);
            logService = this.container.Resolve<ILogService>();
            homeBll = this.container.Resolve<HomeBll>();          
        }
    }
}
