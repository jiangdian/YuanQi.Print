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
        }
        #endregion
    }
}
