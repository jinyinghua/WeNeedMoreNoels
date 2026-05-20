using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    internal class UpdateEnemyInfoEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.UpdateEnemyInfo && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.UpdateEnemyInfo(message.UpdateEnemyInfo);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"UpdateEnemyInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
