using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyGetCoinEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyGetCoin && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            NotifyCoinChanged coin = message.NotifyCoinChanged;
            WNMNTools.GetCoin(coin.PartyID, coin.coinType, coin.count);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"Item:{JsonConvert.SerializeObject(message)}";
        }
    }
}
