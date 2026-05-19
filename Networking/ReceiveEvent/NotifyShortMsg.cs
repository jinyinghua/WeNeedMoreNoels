using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyShortMsg : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyShortMsg && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.SendMsg(message.NotifyShortMsg.ID, message.NotifyShortMsg.key);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"ShortMsg:{JsonConvert.SerializeObject(message)}";
        }
    }
}
