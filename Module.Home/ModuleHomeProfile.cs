using Module.BiaoQian.ViewModels;
using Module.HeGe.ViewModels;
using Module.Home.Bll;
using Module.Home.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using YuanQiTool;
using YuanQiTool.PLC;
using YuanQiTool.PLC.OPCUA;
using YuanQiTool.Scanner.Honeywell;

namespace Module.Home
{
    public class ModuleHomeProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Home>();
            containerRegistry.RegisterSingleton<HomeBll>();
            containerRegistry.RegisterSingleton<BiaoQianViewModel>();
            containerRegistry.RegisterSingleton<HeGeViewModel>();
            containerRegistry.RegisterSingleton<HoneywellTCPScanner>();
            containerRegistry.RegisterSingleton<ILogService, LogService>();
            containerRegistry.Register<IPLCHelper, OPCUAHelper>();
            //containerRegistry.RegisterSingleton<IFreeSqlHelper>(() => new FreeMySqlHelper(ConfigurationManager.AppSettings["SqliteStr"].ToString()));
        }
    }
}
