using System.Collections.Generic;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHostMessage
    {
        public bool InitOther;

        public int ExcludeID;

        public int InitID;

        public List<KeyValuePair<int, ConnectPeerInfo>> PeerInfos;

        public List<KeyValuePair<int, ClientConfig>> PeerConfigs;

        public List<KeyValuePair<int, PartyManager.Party>> PeerParties;
    }

    public class ConnectPeerInfo
    {
        public string IP;
        public int Port;
    }
}
