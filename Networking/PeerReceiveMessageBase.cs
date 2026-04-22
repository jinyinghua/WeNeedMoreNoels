using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking
{
    public abstract class PeerReceiveMessageBase
    {
        public abstract bool CheckMessage(WNMNPeerMessage message);

        public abstract void ReceiveMessage(WNMNPeerMessage message);

        public abstract string ToMessageString(WNMNPeerMessage message);
    }
}
