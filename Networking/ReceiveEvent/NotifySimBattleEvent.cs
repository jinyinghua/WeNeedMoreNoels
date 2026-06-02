using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifySimBattleEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifySimBattle && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.NotifySimBattle(message.PeerId, message.SimBattle);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"SimBattle:{JsonConvert.SerializeObject(message)}";
        }
    }
}
