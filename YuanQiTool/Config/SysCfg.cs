using System;

namespace YuanQiTool
{
    public class SysCfg
    {
        /// <summary>
        /// NO
        /// </summary>
        public static string NO => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// NAME
        /// </summary>
        public static string NAME => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// SCAN_COM
        /// </summary>
        public static string SCAN_COM => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");
        /// <summary>
        /// Print_COM
        /// </summary>
        public static string Print_COM => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// PLC_IP
        /// </summary>
        public static string PLC_IP => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");
        /// <summary>
        /// PLCType
        /// </summary>
        public static string PLCType => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");
        /// <summary>
        /// PLCaddr
        /// </summary>
        public static string PLCaddr => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// PLC_PORT
        /// </summary>
        public static short PLC_PORT => ConfigurationUtil.GetConfiguration(short.Parse, () => (short)0);

        /// <summary>
        /// 托盘到位检测的DM地址
        /// </summary>
        public static short DM_READY => ConfigurationUtil.GetConfiguration(short.Parse, () => (short)0);


        /// <summary>
        /// 打印机端口
        /// </summary>
        public static int PRINTER_PORT => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        public static int PRINTERH_PORT => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        /// <summary>
        /// 打印机IP
        /// </summary>
        public static string PRINTER_IP => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");
        public static string PRINTERH_IP => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");
        /// <summary>
        /// 扫码枪端口
        /// </summary>
        public static int SCAN_PORT => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);

        /// <summary>
        /// 打印机IP
        /// </summary>
        public static string SCAN_IP => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// 打印机DPI
        /// </summary>
        public static int DPI => ConfigurationUtil.GetConfiguration(int.Parse, () => 300);

        public static int HDPI => ConfigurationUtil.GetConfiguration(int.Parse, () => 300);
        /// <summary>
        /// 标签整体偏移X
        /// </summary>
        public static int OffsetX => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        public static int HOffsetX => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        /// <summary>
        /// 标签整体偏移Y
        /// </summary>
        public static int OffsetY => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        public static int HOffsetY => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        /// <summary>
        /// 旋转180
        /// </summary>
        public static bool Rotate180 => ConfigurationUtil.GetConfiguration(bool.Parse, () => false);
        public static bool HRotate180 => ConfigurationUtil.GetConfiguration(bool.Parse, () => true);
        /// <summary>
        /// 截取条码的起始位（1~N）
        /// </summary>
        public static int StartCut => ConfigurationUtil.GetConfiguration(int.Parse, () => 8);

        /// <summary>
        /// 截取条码的结束位（1~N）
        /// </summary>
        public static int EndCut => ConfigurationUtil.GetConfiguration(int.Parse, () => 21);

        /// <summary>
        /// 条码前补字符
        /// </summary>
        public static string CodeBefore => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");


        /// <summary>
        /// 标签顶部(-120~120)，负值可回滚标签
        /// </summary>
        public static int LableTop => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        public static int HLableTop => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);
        public static string HTMBody => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "1");
        public static bool BiaoQian => ConfigurationUtil.GetConfiguration(bool.Parse, () => false);
        public static bool HeGe => ConfigurationUtil.GetConfiguration(bool.Parse, () => false);
        public static bool SetConfiguration(string key, object val)
        {
            return ConfigurationUtil.SetConfiguration(key, val.ToString());
        }
    }
}
