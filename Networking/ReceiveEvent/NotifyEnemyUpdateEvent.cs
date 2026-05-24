using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyEnemyUpdateEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyEnemyUpdate && message.PeerId != WNMNTools.LocalID && DB.IsInBattle;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.NotifyEnemyUpdate(message.NotifyEnemyUpdate, message.PeerId);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"EnemyUpdateInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
