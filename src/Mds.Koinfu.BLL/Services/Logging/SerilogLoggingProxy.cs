using System;
using Serilog;

namespace Mds.Koinfu.BLL.Services.Logging
{
    public class SerilogLoggingProxy : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogLoggingProxy(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public void Log(LogEntry entry)
        {
            switch (entry.Severity)
            {
                case LoggingEventType.Debug:
                    if (entry.Exception != null)
                    {
                        _logger.Debug(entry.Exception, entry.Message);
                    }
                    else
                    {
                        _logger.Debug(entry.Message);
                    }
                    break;
                case LoggingEventType.Information:
                    if (entry.Exception != null)
                    {
                        _logger.Information(entry.Exception, entry.Message);
                    }
                    else
                    {
                        _logger.Information(entry.Message);
                    }
                    break;
                case LoggingEventType.Warning:
                    if (entry.Exception != null)
                    {
                        _logger.Warning(entry.Exception, entry.Message);
                    }
                    else
                    {
                        _logger.Warning(entry.Message);
                    }
                    break;
                case LoggingEventType.Error:
                    if (entry.Exception != null)
                    {
                        _logger.Error(entry.Exception, entry.Message);
                    }
                    else
                    {
                        _logger.Error(entry.Message);
                    }
                    break;
                case LoggingEventType.Fatal:
                    if (entry.Exception != null)
                    {
                        _logger.Fatal(entry.Exception, entry.Message);
                    }
                    else
                    {
                        _logger.Fatal(entry.Message);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
