using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace YuanQiTool.SingletonWindow
{
    public static class SingletonWindow
    {
        public static bool Process()    //如果不适用附加属性也可以直接使用此函数
        {
            //判断单实例的方式有很多，如mutex，process，文件锁等，这里用的是process方式

            var process = GetRunningInstance();
            if (process != null)
            {
                MessageBox.Show("程序已在运行中...", "提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                HandleRunningInstance(process);
                Environment.Exit(0);
                return false;
            }
            return true;
        }

        const int WS_SHOWNORMAL = 1;

        [DllImport("User32.dll")]
        static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        static Process GetRunningInstance()
        {
            var current = System.Diagnostics.Process.GetCurrentProcess();
            var processes = System.Diagnostics.Process.GetProcessesByName(current.ProcessName);

            foreach (var process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (process.MainModule.FileName == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        static void HandleRunningInstance(Process instance)
        {
            if (instance.MainWindowHandle != IntPtr.Zero)
            {
                for (int i = 0; i < 2; i++)
                {
                    FlashWindow(instance.MainWindowHandle, 500);
                }

                SetForegroundWindow(instance.MainWindowHandle);
                ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
            }
            else
            {
                MessageBox.Show("已经有一个实例在运行，无法启动第二个实例");
            }
        }

        static void FlashWindow(IntPtr hanlde, int interval)
        {
            FlashWindow(hanlde, true);
            Thread.Sleep(interval);
            FlashWindow(hanlde, false);
            Thread.Sleep(interval);
        }
    }
}
