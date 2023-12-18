using Prism.Ioc;
using Prism.Modularity;

namespace Module.HeGe
{
    public class ModuleHeGeProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.HeGe>();
            //containerRegistry.RegisterSingleton<HomeBll>();
            //containerRegistry.RegisterSingleton<ILogService, LogService>();
            //containerRegistry.Register<IPLCHelper, S7Helper>();
            //containerRegistry.RegisterSingleton<IFreeSqlHelper>(() => new FreeMySqlHelper(ConfigurationManager.AppSettings["SqliteStr"].ToString()));
        }
    }
}
