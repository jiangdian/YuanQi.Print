using Module.BiaoQian.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using YuanQiTool;

namespace Module.RenGong
{
    public class ModuleRenGongProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.RenGong>("Plan");
            containerRegistry.RegisterSingleton<ILogService, LogService>();
            containerRegistry.RegisterSingleton<BiaoQianViewModel>();
            //containerRegistry.RegisterSingleton<IFreeSqlHelper>(() => new FreeMySqlHelper(ConfigurationManager.AppSettings["SqliteStr"].ToString()));
        }
    }
}
