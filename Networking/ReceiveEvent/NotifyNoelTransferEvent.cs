using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifyNoelTransferEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifyNoelTransfer && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            WNMNTools.TransferMainNoel(message.NotifyNoelTransfer);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"NotifyTransferInfo:{JsonConvert.SerializeObject(message)}";
        }
    }
}
