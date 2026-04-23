using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class UpdateNoelInfoEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.UpdateNoelInfo && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.UpdateNoel(message.PeerId, message.UpdateNoelInfo);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"UpdateInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
