using nel;
using System.Collections.Generic;

namespace WeNeedMoreNoels
{
    public static class DB
    {
        public static bool ShowReceiveDebug = false;

        public static bool ShowLocationDebug = false;

        public static PRNoel MainPR;

        public static bool ShadowAppear;

        public static Dictionary<int, ShadowNoel> noelDics = [];

        public static NetWorkType networkType;

        public const string CONNECTION_ACCESS_KEY = "6db535fbb5ac7e0b031d412a807658f7";
    }
}
