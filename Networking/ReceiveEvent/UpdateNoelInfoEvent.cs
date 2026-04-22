using WeNeedMoreNoels.CSNetworking;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class UpdateNoelInfoEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type != WNMNPeerMessageType.UpdateNoelInfo;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {

        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return "";
        }
    }
}
