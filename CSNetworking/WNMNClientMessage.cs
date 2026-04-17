using Newtonsoft.Json;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNClientMessage
    {
        public int PeerID;

        public WNMNClientMessageType Type;

        public string Content;

        public static WNMNClientMessage ReportLocation(int peerID, ShadowNoelLocation location) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.ReportLocation,
            Content = JsonConvert.SerializeObject(location)
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

        public override string ToString()
        {
            return $"Client#{PeerID} message, type:{Type}, content:{Content}";
        }
    }

    public enum WNMNClientMessageType
    {
        ReportLocation,
        NotifyChangeMapBefore,
        NotifyChangeMapAfter
    }
}
