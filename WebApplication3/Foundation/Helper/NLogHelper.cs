using System;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using LogLevel = NLog.LogLevel;

namespace WebApplication3.Foundation.Helper;

public class NLogHelper
{
    static NLogHelper()
    {
        var config = new LoggingConfiguration();

        // 配置控制台日志输出
        var consoleTarget = new ConsoleTarget("console")
        {
            Layout = "${longdate} ${level:uppercase=true} ${message} ${exception:format=toString,stackTrace}",
            Encoding = Encoding.UTF8
        };
        config.AddTarget(consoleTarget);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);

        // 配置文件日志输出
        var fileTarget = new FileTarget("file")
        {
            Encoding = Encoding.UTF8,
            FileName = "${basedir}/logs/logfile.log",
            Layout = "${longdate} ${level:uppercase=true} ${message} ${exception:format=toString,stackTrace}",
            ArchiveEvery = FileArchivePeriod.Day,
            MaxArchiveFiles = 7
        };
        config.AddTarget(fileTarget);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);

        // 应用配置
        LogManager.Configuration = config;
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static void Info(string message)
    {
        Logger.Info(message);
    }

    public static void Debug(string message)
    {
        Logger.Debug(message);
    }

    public static void Warn(string message)
    {
        Logger.Warn(message);
    }

    public static void Error(string message, Exception ex = null)
    {
        if (ex != null)
        {
            Logger.Error(ex, message);
        }
        else
        {
            Logger.Error(message);
        }
    }

    public static void Fatal(string message, Exception ex = null)
    {
        if (ex != null)
        {
            Logger.Fatal(ex, message);
        }
        else
        {
            Logger.Fatal(message);
        }
    }
}