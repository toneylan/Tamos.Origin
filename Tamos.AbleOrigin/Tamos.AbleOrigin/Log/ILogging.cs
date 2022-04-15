namespace Tamos.AbleOrigin;

public interface ILogging
{
    /*/// <summary>
    /// 配置
    /// </summary>
    void Configure(LoggingOptions options);*/

    void Debug(string message);

    void DebugFormat(string format, params object?[] args);

    void Info(string message);

    void InfoFormat(string format, params object?[] args);

    void Warn(string message);

    void WarnFormat(string format, params object?[] args);

    void Error(string message);

    void ErrorFormat(string format, params object?[] args);
}