using System.Collections.Generic;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHostMessage
    {
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
