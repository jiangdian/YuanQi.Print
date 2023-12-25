## 01 Nuget
Serilog
Serilog.Sinks.Console
Serilog.Sinks.File
## 02 注入
```c#
string logOutputTemplate = "{Timestamp:HH:mm:ss.fff zzz} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}";
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .MinimumLevel.Override("Default", LogEventLevel.Information)
              .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
              .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
              .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
              .Enrich.FromLogContext()
              .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
              .WriteTo.File($"{AppContext.BaseDirectory}Logs/logs.log", rollingInterval: RollingInterval.Day, outputTemplate: logOutputTemplate)
              .CreateLogger();
```
## 03 Log4Net配置
```xml
<configSections>
		<section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
	</configSections>
	<!--日志配置部分-->
	<log4net>
		<appender name="FileAppender" type="log4net.Appender.RollingFileAppender">
			<param name="Encoding" value="utf-8" />
			<file value="log\" />
			<appendToFile value="true" />
			<rollingStyle value="Date" />
			<datePattern value="yyyyMM/yyyyMMdd&quot;.log&quot;" />
			<StaticLogFileName value="false" />
			<layout type="log4net.Layout.PatternLayout">
				<conversionPattern value="%date [%thread] %-5level- %message%newline" />
			</layout>
			<!--多线程写入支持-->
			<lockingModel type="log4net.Appender.FileAppender+MinimalLock" />
		</appender>
		<root>
			<level value="DEBUG" />
			<appender-ref ref="ColoredConsoleAppender" />
			<appender-ref ref="FileAppender" />
		</root>
	</log4net>
```