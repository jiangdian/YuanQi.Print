using log4net;
using log4net.Config;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace YuanQiTool
{

    public  class LogService : ILogService
        {
            private static LogService _instance;
            private ILog log;
            bool isDebug = true;
            public LogService()
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location+ ".Config"));
                this.log = LogManager.GetLogger(typeof(LogService));
             }

            static Object obj = new object();
            public static LogService Instance
            {
                get
                {
                    lock (obj)
                    {
                        if (LogService._instance == null)
                        {
                            LogService._instance = new LogService();
                        }
                        return LogService._instance;
                    }
                }
            }

            public void Debug(object message)
            {
                if (isDebug)
                {
                    this.log.Debug(message);
                }

            }
            public void DebugFormatted(string format, params object[] args)
            {
                if (isDebug)
                {
                    this.log.DebugFormat(CultureInfo.InvariantCulture, format, args);
                }
            }

            public void Info(object message)
            {
                this.log.Info(message);
            }

            public void InfoFormatted(string format, params object[] args)
            {
                this.log.InfoFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void Warn(object message, [CallerMemberName] string key = "")
            {
                this.log.Warn(key);
                this.log.Warn(message);
            }

            public void Warn(object message, Exception exception, [CallerMemberName] string key = "")
            {
                this.log.Warn(key);
                this.log.Warn(message, exception);
            }

            public void WarnFormatted(string format, params object[] args)
            {
                this.log.WarnFormat(CultureInfo.InvariantCulture, format, args);
            }


            public void Error(object message, [CallerMemberName] string key = "")
            {
                this.log.Error(key);
                this.log.Error(message);
            }

            public void Error(object message, Exception exception, [CallerMemberName] string key = "")
            {
                this.log.Error(key);
                this.log.Error(message, exception);
            }

            public void ErrorFormatted(string format, params object[] args)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
            }

            public void Fatal(object message, [CallerMemberName] string key = "")
            {
                this.log.Fatal(key);
                this.log.Fatal(message);
            }

            public void Fatal(object message, Exception exception, [CallerMemberName] string key = "")
            {
                this.log.Fatal(key);
                this.log.Fatal(message, exception);
            }


            public void FatalFormatted(string format, params object[] args)
            {
                this.log.FatalFormat(CultureInfo.InvariantCulture, format, args);
            }

            public bool IsDebugEnabled
            {
                get
                {
                    return this.log.IsDebugEnabled;
                }
            }

            public bool IsInfoEnabled
            {
                get
                {
                    return this.log.IsInfoEnabled;
                }
            }

            public bool IsWarnEnabled
            {
                get
                {
                    return this.log.IsWarnEnabled;
                }
            }

            public bool IsErrorEnabled
            {
                get
                {
                    return this.log.IsErrorEnabled;
                }
            }

            public bool IsFatalEnabled
            {
                get
                {
                    return this.log.IsFatalEnabled;
                }
            }

            /// <summary>
            /// 删除N天前日志文件
            /// </summary>
            /// <param name="days"></param>
            public void DeleteLogFile(ushort days = 30)
            {
                try
                {
                    //文件夹路径
                    string strFolderPath = Environment.CurrentDirectory + "\\Log\\";
                    DirectoryInfo dyInfo = new DirectoryInfo(strFolderPath);
                    //获取文件夹下所有的文件
                    foreach (FileInfo feInfo in dyInfo.GetFiles())
                    {
                        //判断文件日期是否几天前创建，是则删除
                        if (feInfo.CreationTime < DateTime.Today.AddDays(-1 * days))
                            feInfo.Delete();
                    }
                }
                catch (Exception ex)
                {
                    this.Error("删除日志文件发生错误:" + ex.Message);
                }
            }
        }
    }

