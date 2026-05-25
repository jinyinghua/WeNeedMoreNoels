using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyNoelStartBattleEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyNoelStartBattle && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            if (message.Battle.isSim)
            {
                ShadowNoelExtensions.StartSimBattle(message.PeerId);
            }
            else
            {
                ShadowNoelExtensions.StartCurMapBattle(message.Battle.key, message.PeerId);
            }
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"Notify start battle, info:{JsonConvert.SerializeObject(message)}";
        }
    }
}
