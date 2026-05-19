using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyGetItemEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyGetItem && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            NotifyItemChanged item = message.NotifyItemChanged;
            WNMNTools.GetItem(item.PartyID, item.key, item.count, item.grade);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"Item:{JsonConvert.SerializeObject(message)}";
        }
    }
}
