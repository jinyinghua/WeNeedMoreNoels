using BepInEx.Logging;
using System.Collections.Generic;

namespace WeNeedMoreNoels
{
    public static class WNMNConsole
    {
        static ILogListener _originListener;

        public static List<LogEventArgs> LogList;

        public static void Init()
        {
            //_originListener = Logger.Listeners.ElementAt(0);
            //Logger.Listeners.Clear();
            //Logger.Listeners.Add(new WNMNConsoleListener());
        }
    }
}
