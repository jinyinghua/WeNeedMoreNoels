using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class InitNoelEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.InitNoel && message.PeerId != WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            IniConfig config = message.InitNoelConfig;
            ShadowNoelExtensions.GenerateShadowNoel(config.ClientConfig, config.Id);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"InitNoel:{JsonConvert.SerializeObject(message)}";
        }
    }
}
