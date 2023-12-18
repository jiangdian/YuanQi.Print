using Prism.Ioc;
using Prism.Modularity;

namespace Module.Setting
{
    public class ModuleSettingProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Setting>();
        }
    }
}
