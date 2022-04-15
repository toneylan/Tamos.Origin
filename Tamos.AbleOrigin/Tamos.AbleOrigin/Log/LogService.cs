namespace Tamos.AbleOrigin;

/// <summary>
/// 集中日志配置：ExternalService/CentralLog_ApiKey、CentralLog_ServerUrl
/// </summary>
public static class LogService
{
    #region Init Logger

    private static ILogging? _logger;

    public static ILogging Logger => _logger ??= (ServiceLocator.GetOrReflect<ILogging>("Log4netIntegration") ?? NullLogger.Instance);
        
    /// <summary>
    /// 设置ILogging实现，由Booster调用。
    /// </summary>
    public static void SetProvider(ILogging logging)
    {
        _logger = logging;
    }
        
    #endregion

    /// <summary>
    /// 防止Logger初始化失败时，无法写入日志。也避免初始化与写日志的死循环。
    /// </summary>
    internal static void SafeWrite(string errorMsg)
    {
        if (_logger == null) NullLogger.Instance.Error(errorMsg);
        else _logger.Error(errorMsg);
    }

    #region Write log

    public static void Debug(string message)
    {
        Logger.Debug(message);
    }

    public static void DebugFormat(string format, params object?[] args)
    {
        Logger.DebugFormat(format, args);
    }

    public static void Info(string message)
    {
        Logger.Info(message);
    }

    public static void InfoFormat(string format, params object?[] args)
    {
        Logger.InfoFormat(format, args);
    }

    public static void Warn(string message)
    {
        Logger.Warn(message);
    }

    public static void WarnFormat(string format, params object?[] args)
    {
        Logger.WarnFormat(format, args);
    }

    public static void Error(string message)
    {
        Logger.Error(message);
    }

    public static void ErrorFormat(string format, params object?[] args)
    {
        Logger.ErrorFormat(format, args);
    }

    public static void Error(Exception exception)
    {
        Logger.Error(exception.ToString());
    }

    #endregion
}