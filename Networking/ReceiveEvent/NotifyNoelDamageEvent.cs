using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyNoelDamageEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyNoelDamage && message.PeerId == WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            ShadowNoelExtensions.DamageNoel(message.PeerId, message.NotifyNoelDamage);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"DmageInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
