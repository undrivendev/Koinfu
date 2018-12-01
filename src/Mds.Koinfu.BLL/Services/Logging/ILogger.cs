using System;
namespace Mds.Koinfu.BLL.Services.Logging
{
    public interface ILogger
    {
        void Log(LogEntry entry);
    }
}
