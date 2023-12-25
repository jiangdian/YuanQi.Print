using System;
using System.Runtime.CompilerServices;

namespace YuanQiTool
{
    public interface ILogService
    {
        void Debug(object message);
        void DebugFormatted(string format, params object[] args);
        void Info(object message);
        void InfoFormatted(string format, params object[] args);
        void Warn(object message, [CallerMemberName] string key = "");
        void Warn(object message, Exception exception, [CallerMemberName] string key = "");
        void WarnFormatted(string format, params object[] args);
        void Error(object message, [CallerMemberName] string key = "");
        void Error(object message, Exception exception, [CallerMemberName] string key = "");
        void ErrorFormatted(string format, params object[] args);
        void Fatal(object message, [CallerMemberName] string key = "");
        void Fatal(object message, Exception exception, [CallerMemberName] string key = "");
        void FatalFormatted(string format, params object[] args);
        void DeleteLogFile(ushort days = 30);
    }
}
