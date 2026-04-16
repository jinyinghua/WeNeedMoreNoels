using Newtonsoft.Json;
using System.Collections.Generic;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHostMessage
    {
        public WNMNHostMessageType Type;

        public string Content;

        public static WNMNHostMessage Init(int id) => new()
        {
            Type = WNMNHostMessageType.Init,
            Content = id.ToString()
        };

        public static WNMNHostMessage UpdateLocation(ShadowNoelLocation hostLocation, Dictionary<int, ShadowNoelLocation> peerLocations) => new()
        {
            Type = WNMNHostMessageType.UpdateLocation,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<ShadowNoelLocation>()
            {
                HostContent = hostLocation
            })
        };

        public override string ToString()
        {
            return $"Host message, type:{Type}, content:{Content}";
        }
    }

    public enum WNMNHostMessageType
    {
        Init,
        UpdateLocation
    }
}
