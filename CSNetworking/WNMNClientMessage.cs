using nel;
using Newtonsoft.Json;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNClientMessage
    {
        public int PeerID;

        public WNMNClientMessageType Type;

        public string Content;

        public static WNMNClientMessage Init(int peerID, WNMNTools.NetworkConfig config) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.Init,
            Content = JsonConvert.SerializeObject(config)
        };

        public static WNMNClientMessage ReportInfo(int peerID, ShadowNoelInfo info) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.ReportInfo,
            Content = JsonConvert.SerializeObject(info)
        };

        public static WNMNClientMessage NotifyChangeMapBefore(int peerID) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.NotifyChangeMapBefore,
        };

        public static WNMNClientMessage NotifyChangeMapAfter(int peerID, string mpKey) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.NotifyChangeMapAfter,
            Content = mpKey
        };

        public static WNMNClientMessage NotifyStateChange(int peerID, PR.STATE STATE) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.NotifyStateChange,
            Content = ((int)STATE).ToString()
        };

        public static WNMNClientMessage NotifyNoelDamage(int peerID, ShadowNoelDamage Atk) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.NotifyNoelDamage,
            Content = JsonConvert.SerializeObject(Atk)
        };

        public override string ToString()
        {
            return $"Client#{PeerID} message, type:{Type}, content:{Content}";
        }
    }

    public enum WNMNClientMessageType
    {
        Init,
        ReportInfo,
        NotifyChangeMapBefore,
        NotifyChangeMapAfter,
        NotifyStateChange,
        NotifyNoelDamage
    }
}
