using Newtonsoft.Json;
using System.Numerics;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNClientMessage
    {
        public int PeerID;

        public WNMNClientMessageType Type;

        public string Content;

        public static WNMNClientMessage ReportLocation(int peerID, Vector2 position) => new()
        {
            PeerID = peerID,
            Type = WNMNClientMessageType.ReportLocation,
            Content = JsonConvert.SerializeObject(position)
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
