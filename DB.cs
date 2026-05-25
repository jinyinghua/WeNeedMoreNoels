using nel;
using System.Collections.Generic;
using WeNeedMoreNoels.CSNetworking;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels
{
    public static class DB
    {
        public static int MaxPlayerCount = 5;

        public static bool WNMNUIClicking = false;

        public static NetWorkType WNMNEnterNetworkType;

        public static bool WNMNEnterNetworkTypeSelected = false;

        public static bool WNMNHostSelectSVD = false;

        public static bool WNMNClientTransferNotComplete = false;

        public static bool WNMNHostClosed = false;

        public static bool WNMNHostKicked = false;

        public static bool ShowReceiveDebug = false;

        public static bool ShowLocationDebug = false;

        public static bool PreloadResource;

        public static PRNoel MainPR;

        public static ShadowNoelNickname MainPRNickname;

        public static ShadowNoelNickname MainPRMsg;

        public static bool ShadowAppear;

        public static string Nickname;

        public static byte[] SyncSaveContentBuffer;

        public static byte[] SyncSmnContentBuffer;

        public static int LocalNoelParty;

        public static Dictionary<int, ShadowNoelInstance> noelIns = [];

        public static Dictionary<int, PartyManager.Party> partyInfos = [];

        public static Dictionary<int, ConnectPeerInfo> peerInfos = [];

        public static Dictionary<int, ClientConfig> peerConfigs = [];

        public static Dictionary<MagicItem, ShadowNoel> MNBridge = [];

        public static List<NelEnemy> CurEnemies = [];

        public static float MovementEpslion = 0.001f;

        public static NetWorkType networkType;

        public const string CONNECTION_ACCESS_KEY = "6db535fbb5ac7e0b031d412a807658f7";

        public const string TRANSFER_ACCESS_KEY = "424fe8bd2741b4a4d44411e07c48edff";

        public const string P2P_ACCESS_KEY = "3f3dd28a3b54d0531faa87028b37e590";

        public const string SYNC_FILE_NAME = "wnmn_sync.aicsave";

        public static string Plugin_local_path;

        public static string Game_streaming_asset;

        public static WNMNTools.NetworkConfig InitConfig = null;

        public static bool IsMultiplayer => InitConfig != null;

        public static bool Test;

        public static float Test1;

        public static bool IsMainPR;

        public static float LocalMagicAim;

        public static M2LpSummon CurSummoner;

        public static bool Mute;

        public static bool IsInBattle;

        public static Dictionary<int, EnemySynchronizerSyncClient> SyncClients = [];

        public static Dictionary<int, List<EnemySynchronizerSyncClient>> peerClients = [];

        public static Dictionary<int, EnemySynchronizerSyncHost> SyncHosts = [];

        public static HashSet<string> StartedBattleSummonerKeys = [];

        public static bool StartedSimBattle;

        public static Dictionary<int, int> peerDelays = [];
    }
}
