using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class ApplyNoelDamageEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyNoelDamage && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            ShadowNoelExtensions.DamageNoel(message.PeerId, message.UpdateNoelDamage);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"DmageInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
