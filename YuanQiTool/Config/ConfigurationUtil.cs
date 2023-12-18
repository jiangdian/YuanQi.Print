using System;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;

namespace YuanQiTool
{
    internal class ConfigurationUtil
    {
        public static string GetConfiguration([CallerMemberName] string key = "")
        {
            string rawConfigValue = ConfigurationManager.AppSettings[key];
            return rawConfigValue;
        }
        public static T GetConfiguration<T>(Func<string, T> parseFunc, Func<T> defaultTValueFunc, [CallerMemberName] string key = "")
        {
            try
            {
                string rawConfigValue = GetConfiguration(key);
                return !string.IsNullOrEmpty(rawConfigValue) ?
                        parseFunc(rawConfigValue) :
                        defaultTValueFunc();
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("[LOG]GetConfiguration Error！",ex);
                return default(T);
            }
        }
        public static bool SetConfiguration(string key, string value)
        {
            try
            {
                // Get the configuration file. 
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings.AllKeys.Contains(key))
                {
                    config.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, value));
                }
                // Save the configuration file. 
                config.AppSettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);
                // Force a reload of the changed section. 
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("[LOG]SetConfiguration Error！", ex);
                return false;
            }
        }
    }
}
