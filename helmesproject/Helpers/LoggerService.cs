using System.Text;

namespace helmesproject.Helpers
{
    public class LoggerService
    {
        private  readonly IConfiguration _configuration;
        private readonly string _loggingPath;
        private readonly string _minLoggingLevel;

        public LoggerService(IConfiguration configuration)
        {
            _configuration = configuration;
            _loggingPath = _configuration.GetSection("LoggingPath").Value;
            _minLoggingLevel = _configuration.GetSection("MinLogLevel").Value;

        }
        public void Log(string message, LogLevel level,string? stackTrace = null, string? info1 = null, string? info2 = null)
        {
            LogLevel minLogLevel;
            bool log = true;
            string logType = "all";

            if (Enum.TryParse(_minLoggingLevel, out minLogLevel)) {
                if (minLogLevel > level)
                    log = false;
            }

            if (log)
            {
                if (level == LogLevel.Error)
                    logType = "error";

                string currentDate = DateTime.Today.ToString("yyMMdd");
                string filename = $"{_loggingPath}/{logType}-log{currentDate}.txt";
                StringBuilder sb = new StringBuilder($"{DateTime.Now.ToString("[HH:mm:ss.ffff]")} [{level}] {message}; ");

                if (!String.IsNullOrEmpty(stackTrace))
                    sb.Append($"stack: { stackTrace}; ");
                if (!String.IsNullOrEmpty(info1))
                    sb.Append($"info1: {info1}; ");
                if (!String.IsNullOrEmpty(info2))
                    sb.Append($"info2: {info2}; ");

                sb.Append(Environment.NewLine);

                File.AppendAllText(filename, sb.ToString());
            }
        }
    }
}
