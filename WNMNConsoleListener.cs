using BepInEx.Logging;

namespace WeNeedMoreNoels
{
    public class WNMNConsoleListener : ILogListener
    {
        public LogLevel LogLevelFilter => LogLevel.All;

        void ILogListener.LogEvent(object sender, LogEventArgs eventArgs)
        {

        }

        public void Dispose()
        {

        }
    }
}
