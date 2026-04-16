using Newtonsoft.Json;

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

        public override string ToString()
        {
            return $"Client#{PeerID} message, type:{Type}, content:{Content}";
        }
    }

    public enum WNMNClientMessageType
    {
        ReportLocation
    }
}
