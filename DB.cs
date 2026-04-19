using nel;
using System.Collections.Generic;

namespace WeNeedMoreNoels
{
    public static class DB
    {
        public static bool WNMNUIClicking = false;

        public static NetWorkType WNMNEnterNetworkType;

        public static bool WNMNEnterNetworkTypeSelected = false;

        public static bool WNMNHostSelectSVD = false;

        public static bool WNMNClientTransferNotComplete = false;

        public static bool WNMNHostClosed = false;

        public static bool ShowReceiveDebug = false;

        public static bool ShowLocationDebug = false;

        public static bool PreloadResource;

        public static PRNoel MainPR;

        public static bool ShadowAppear;

        public static string Nickname;

        public static byte[] SyncSaveContentBuffer;

        public static Dictionary<int, string> noelNicknames = [];

        public static Dictionary<int, ShadowNoel> noelDics = [];

        public static Dictionary<int, string> noelMpKeys = [];

        public static Dictionary<int, bool> noelEnables = [];

        public static Dictionary<int, WNMNTools.NetworkConfig> noelConfigs = [];

        public static float MovementEpslion = 0.001f;

        public static NetWorkType networkType;

        public const string CONNECTION_ACCESS_KEY = "6db535fbb5ac7e0b031d412a807658f7";

        public const string TRANSFER_ACCESS_KEY = "424fe8bd2741b4a4d44411e07c48edff";

        public const string SYNC_FILE_NAME = "wnmn_sync.aicsave";

        public static string Plugin_local_path;

        public static string Game_streaming_asset;

        public static WNMNTools.NetworkConfig InitConfig = null;
    }
}
