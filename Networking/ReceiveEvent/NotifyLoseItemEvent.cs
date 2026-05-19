using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyLoseItemEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyLoseItem && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            NotifyItemChanged item = message.NotifyItemChanged;
            WNMNTools.LoseItem(item.PartyID, item.key, item.count, item.grade);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"Item:{JsonConvert.SerializeObject(message)}";
        }
    }
}
