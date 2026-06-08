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
                int x, y;
                if (message.Battle.SpawnPoints.ContainsKey(WNMNTools.LocalID))
                {
                    x = message.Battle.SpawnPoints[WNMNTools.LocalID].x;
                    y = message.Battle.SpawnPoints[WNMNTools.LocalID].y;
                }
                else
                {
                    x = message.Battle.SpawnPoints[-1].x;
                    y = message.Battle.SpawnPoints[-1].y;
                }
                ShadowNoelExtensions.StartSimBattle(message.PeerId, x, y);
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
