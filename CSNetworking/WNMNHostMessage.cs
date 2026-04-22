using System.Collections.Generic;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHostMessage
    {
        public int InitID;

        public List<KeyValuePair<int, ConnectPeerInfo>> PeerInfos;
    }

    public class ConnectPeerInfo
    {
        public string IP;
        public int Port;
    }
}
