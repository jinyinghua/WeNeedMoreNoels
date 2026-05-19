using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyRoomUpdateEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyRoomUpdate && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.UpdateRoomConfig(message.NotifyRoomUpdate);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"Room:{JsonConvert.SerializeObject(message)}";
        }
    }
}
