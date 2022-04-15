using Exceptionless;
using Exceptionless.Log4net;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Tamos.AbleOrigin.Booster;

internal class Log4netIntegration : ILogging
{
    #region Static Init config

    private const string LogDir = "Logs";
    private static readonly ILoggerRepository LogRepository = LogManager.CreateRepository("Log4Repository");
        
    static Log4netIntegration()
    {
        try
        {
            var confFile = new FileInfo(HostApp.GetPath("log4net.config"));
            if (confFile.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(LogRepository, confFile);
            }
            else
            {
                Configure();
            }

            //检查Logs目录是否存在。docker下测试时无法自动创建。
            var logPath = HostApp.GetPath(LogDir);
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static ILayout GetLayout()
    {
        var patternLayout = new PatternLayout {ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"};
        patternLayout.ActivateOptions();

        return patternLayout;
    }

    //默认配置代码
    private static void Configure()
    {
        #region Appender

        RollingFileAppender CreateAppender(string file, int maxBackups) => new RollingFileAppender
        {
            Name = $"{file}Appender",
            File = HostApp.GetPath($"{LogDir}{Path.DirectorySeparatorChar}{file}.log"),
            RollingStyle = RollingFileAppender.RollingMode.Composite,
            DatePattern = "yyyy-MM-dd'.log'",
            MaximumFileSize = "200MB",
            MaxSizeRollBackups = maxBackups,
            //StaticLogFileName = false,
            AppendToFile = true,
            LockingModel = new FileAppender.MinimalLock(),
            Layout = GetLayout()
        };

        var errorAppender = CreateAppender("error", 60);
        errorAppender.AddFilter(new LevelRangeFilter
        {
            LevelMin = Level.Error,
            LevelMax = Level.Fatal
        });
        errorAppender.ActivateOptions();

        var infoAppender = CreateAppender("info", 30);
        infoAppender.AddFilter(new LevelRangeFilter
        {
            LevelMin = Level.Info,
            LevelMax = Level.Warn
        });
        infoAppender.ActivateOptions();

        var debugAppender = CreateAppender("debug", 30);
        debugAppender.AddFilter(new LevelMatchFilter {LevelToMatch = Level.Debug});
        debugAppender.AddFilter(new DenyAllFilter());
        debugAppender.ActivateOptions();
            
        var hierarchy = (Hierarchy) LogRepository;
        hierarchy.Root.AddAppender(errorAppender);
        hierarchy.Root.AddAppender(infoAppender);
        hierarchy.Root.AddAppender(debugAppender);
            
        #endregion
            
        hierarchy.Root.Level = Level.Debug;
        hierarchy.Configured = true;
    }

    /// <summary>
    /// 添加集中日志功能
    /// </summary>
    internal static void AddCentralLog()
    {
        //--Exceptionless
        var apiKey = ServiceAddressConfig.GetExternalSvcSet("CentralLog_ApiKey");
        if (apiKey.IsNull()) return;
        var serverUrl = ServiceAddressConfig.GetExternalSvcSet("CentralLog_ServerUrl", "http://host.docker.internal:5000");

        ExceptionlessClient.Default.Configuration.ServerUrl = serverUrl;
        ExceptionlessClient.Default.Configuration.ApiKey = apiKey;
        var excLessAppender = new ExceptionlessAppender
        {
            Name = "ExceptionlessLog",
            ServerUrl = serverUrl,
            ApiKey = apiKey,
            Layout = GetLayout()
        };
        excLessAppender.ActivateOptions();

        ((Hierarchy) LogRepository).Root.AddAppender(excLessAppender);
    }
        
    #endregion

    #region Ctor

    private readonly ILog _log;

    public Log4netIntegration(string? name = null)
    {
        _log = LogManager.GetLogger(LogRepository.Name, name ?? nameof(Log4netIntegration));
    }
        
    public ILogging GetLogging(string name)
    {
        return new Log4netIntegration(name);
    }
        
    #endregion

    #region Write log

    public void Debug(string message)
    {
        _log.Debug(message);
    }

    public void DebugFormat(string format, params object?[] args)
    {
        _log.DebugFormat(format, args);
    }

    public void Info(string message)
    {
        _log.Info(message);
    }

    public void InfoFormat(string format, params object?[] args)
    {
        _log.InfoFormat(format, args);
    }

    public void Warn(string message)
    {
        _log.Warn(message);
    }

    public void WarnFormat(string format, params object?[] args)
    {
        _log.WarnFormat(format, args);
    }

    public void Error(string message)
    {
        _log.Error(message);
    }

    public void ErrorFormat(string format, params object?[] args)
    {
        _log.ErrorFormat(format, args);
    }

    #endregion
}