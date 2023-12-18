using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Diagnostics;
using System.Windows;
using YuanQiTool;
using YuanQiTool.SingletonWindow;
using YuanQiUI.Views;

namespace YuanQiPrint
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            if (SingletonWindow.Process())
            {
                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                return Container.Resolve<MainWindow>();
            }
            return null;
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogService.Instance.Error("未处理的异常！", e.Exception);
            e.Handled = true;
            MessageBox.Show(e.Exception.Message + e.Exception.StackTrace);
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogService.Instance.Error("未处理的异常！", ex.Message);
            Process.GetCurrentProcess().Kill();
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Module.BiaoQian.ModuleBiaoQianProfile>();
            moduleCatalog.AddModule<Module.Home.ModuleHomeProfile>();
            moduleCatalog.AddModule<Module.HeGe.ModuleHeGeProfile>();
            moduleCatalog.AddModule<Module.Setting.ModuleSettingProfile>();
            moduleCatalog.AddModule<Module.RenGong.ModuleRenGongProfile>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 在应用程序退出时执行的逻辑，例如关闭后台任务、保存数据等
            Environment.Exit(0);
        }
    }
}
