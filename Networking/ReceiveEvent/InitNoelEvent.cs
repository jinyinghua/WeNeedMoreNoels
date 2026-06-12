using Newtonsoft.Json;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class InitNoelEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.InitNoel && message.PeerId != WNMNTools.LocalID && !DB.noelIns.ContainsKey(message.PeerId);
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            IniConfig config = message.InitNoelConfig;
            DB.partyInfos.Add(message.PeerId, config.PartyConfig);
            ShadowNoelExtensions.GenerateShadowNoel(config.ClientConfig, config.Id);
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"InitNoel:{JsonConvert.SerializeObject(message)}";
        }
    }
}
