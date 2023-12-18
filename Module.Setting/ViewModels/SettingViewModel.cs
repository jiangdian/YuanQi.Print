using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using YuanQiTool;

namespace Module.Setting.ViewModels
{
    public  class SettingViewModel : BindableBase
    {
        #region 属性
        private bool _BiaoQian;
        public bool BiaoQian
        {
            get { return _BiaoQian; }
            set 
            { 
                SetProperty(ref _BiaoQian, value);
                SysCfg.SetConfiguration("BiaoQian", value);
            }
        }
        private bool _HeGe;
        public bool HeGe
        {
            get { return _HeGe; }
            set
            { 
                SetProperty(ref _HeGe, value);
                SysCfg.SetConfiguration("HeGe",value);
            }
        }

        #endregion
        #region Command
        /// <summary>
        /// Load 加载配置项
        /// </summary>
        private DelegateCommand _LoadCfgCmd;
        public DelegateCommand LoadCfgCmd =>
            _LoadCfgCmd ?? (_LoadCfgCmd = new DelegateCommand(LoadCfg));
        public  void LoadCfg()
        {
            HeGe = SysCfg.HeGe;
            BiaoQian = SysCfg.BiaoQian;
        }
        /// <summary>
        /// Save 保存配置项
        /// </summary>
        private DelegateCommand _SavePeiZhi;
        public DelegateCommand SavePeiZhi =>
            _SavePeiZhi ?? (_SavePeiZhi = new DelegateCommand(SaveAll));
        public void SaveAll()
        {
            //SysCfg.SetConfiguration("SCAN_IP", SysCfg.SCAN_IP);
            //SysCfg.SetConfiguration("SCAN_PORT", SysCfg.SCAN_PORT);
            //SysCfg.SetConfiguration("PLC_IP", SysCfg.PLC_IP);
            //SysCfg.SetConfiguration("PLC_PORT", SysCfg.PLC_PORT);
            //SysCfg.SetConfiguration("PRINTER_IP", SysCfg.PRINTER_IP);
            //SysCfg.SetConfiguration("PRINTER_PORT", SysCfg.PRINTER_PORT);
            //SysCfg.SetConfiguration("PRINTERH_IP", SysCfg.PRINTERH_IP);
            //SysCfg.SetConfiguration("PRINTERH_PORT", SysCfg.PRINTERH_PORT);
            //SysCfg.SetConfiguration("OffsetX", SysCfg.OffsetX);
            //SysCfg.SetConfiguration("OffsetY", SysCfg.OffsetY);
            //SysCfg.SetConfiguration("DPI", SysCfg.DPI);
            //SysCfg.SetConfiguration("Rotate180", SysCfg.Rotate180);
            //SysCfg.SetConfiguration("StartCut", SysCfg.StartCut);
            //SysCfg.SetConfiguration("EndCut", SysCfg.EndCut);
            //SysCfg.SetConfiguration("CodeBefore", SysCfg.CodeBefore);
            //SysCfg.SetConfiguration("LableTop", SysCfg.LableTop);
            //SysCfg.SetConfiguration("HOffsetX", SysCfg.HOffsetX);
            //SysCfg.SetConfiguration("HOffsetY", SysCfg.HOffsetY);
            //SysCfg.SetConfiguration("HDPI", SysCfg.HDPI);
            //SysCfg.SetConfiguration("HRotate180", SysCfg.HRotate180);
            //SysCfg.SetConfiguration("HLableTop", SysCfg.HLableTop);
            //SysCfg.SetConfiguration("HTMBody", SysCfg.HTMBody);
            //SysCfg.SetConfiguration("HeGe", SysCfg.HeGe);
            //SysCfg.SetConfiguration("BiaoQian", SysCfg.BiaoQian);
        }
        #endregion
    }
}
