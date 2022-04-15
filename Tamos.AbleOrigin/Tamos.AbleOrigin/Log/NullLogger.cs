using System.IO;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 没有日志实现类时的，空实现
    /// </summary>
    internal class NullLogger : ILogging
    {
        internal static readonly NullLogger Instance = new NullLogger();

        private readonly string LogsDir;

        public NullLogger()
        {
            //检查Logs目录是否存在。
            LogsDir = HostApp.GetPath("Logs");
            if (!Directory.Exists(LogsDir)) Directory.CreateDirectory(LogsDir);
        }

        public void Debug(string message)
        {
            WriteLine(message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            WriteLine(format, args);
        }

        public void Info(string message)
        {
            WriteLine(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            WriteLine(format, args);
        }

        public void Warn(string message)
        {
            WriteLine(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            WriteLine(format, args);
        }

        public void Error(string message)
        {
            WriteLine(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            WriteLine(format, args);
        }
        
        private void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        private void WriteLine(string message)
        {
            File.AppendAllText(Path.Combine(LogsDir, "nulllog.txt"), message + "\n");
        }
    }
}