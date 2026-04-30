using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyNoelMagicEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyNoelMagic && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            ShadowNoelExtensions.SetNoelMagic(message.PeerId, message.NotifyNoelMagic);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"MagicInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
