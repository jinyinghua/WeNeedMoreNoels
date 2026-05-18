using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class UpdatePeerInfoEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.UpdatePeerInfo && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.UpdatePeer(message.PeerId, message.UpdatePeerInfo);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"UpdatePeerInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
