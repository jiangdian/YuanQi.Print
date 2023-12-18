using Prism.Mvvm;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Regions;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;

namespace YuanQiUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region 属性
        private readonly IRegionManager regionManager;
        public ObservableCollection<HamburgerMenuIconItem> Menu { get; } = new ObservableCollection<HamburgerMenuIconItem>();
        public ObservableCollection<HamburgerMenuIconItem> OptionsMenu { get; } =new ObservableCollection<HamburgerMenuIconItem>();
        private string _title = string.Empty;
        public string Title
        {
            get 
            { 
                return _title; 
            }
            set
            { 
                SetProperty(ref _title, value);
            }
        }
        #endregion
        #region Command
        private DelegateCommand<HamburgerMenuIconItem> _open;
        public DelegateCommand<HamburgerMenuIconItem> Open =>
            _open ?? (_open = new DelegateCommand<HamburgerMenuIconItem>(OpenCommand));

        void OpenCommand(HamburgerMenuIconItem menuItem)
        {
            regionManager.Regions["ContentRegion"].RequestNavigate(menuItem.Tag.ToString());
        }
        private DelegateCommand _load;
        public DelegateCommand LoadCommand =>
            _load ?? (_load = new DelegateCommand(ExecuteLoadCommand));

        void ExecuteLoadCommand()
        {
            regionManager.Regions["ContentRegion"].RequestNavigate("Home");
        }
        #endregion
        
        public MainWindowViewModel(IRegionManager regionManager)
        {          
            this.regionManager = regionManager;
            Title = ConfigurationManager.AppSettings["AppName"].ToString();

            var Menu = JsonConvert.DeserializeObject<List<MenuItem>>(ConfigurationManager.AppSettings["Menu"].ToString());
            foreach (var item in Menu)
            {
                this.Menu.Add(new HamburgerMenuIconItem()
                {
                    Icon = item.Kind,
                    Label = item.Name,
                    Tag = item.Tag
                });
            }
            var ListOptionsMenu = JsonConvert.DeserializeObject<List<MenuItem>>(ConfigurationManager.AppSettings["OptionsMenu"].ToString());
            foreach (var item in ListOptionsMenu)
            {
                this.OptionsMenu.Add(new HamburgerMenuIconItem()
                {
                    Icon = item.Kind,
                    Label = item.Name,
                    Tag = item.Tag
                });
            }
        }
    }
}
