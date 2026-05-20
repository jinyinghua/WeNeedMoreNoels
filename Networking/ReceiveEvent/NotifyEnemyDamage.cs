using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyEnemyDamageEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyEnemyDamage && message.PeerId == WNMNTools.LocalID && WNMNTools.Type == NetWorkType.Host;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"EnemyDamageInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
