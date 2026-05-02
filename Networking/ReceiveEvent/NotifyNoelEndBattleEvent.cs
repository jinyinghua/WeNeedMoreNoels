using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyNoelEndBattleEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyNoelEndBattle && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            ShadowNoelExtensions.EndCurMapBattle();
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"Notify end battle, info:{JsonConvert.SerializeObject(message)}";
        }
    }
}
