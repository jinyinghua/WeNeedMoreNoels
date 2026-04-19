using nel;
using Newtonsoft.Json;
using System.Collections.Generic;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHostMessage
    {
        public WNMNHostMessageType Type;

        public string Content;

        public static WNMNHostMessage Init(int id, WNMNTools.NetworkConfig config) => new()
        {
            Type = WNMNHostMessageType.Init,
            Content = JsonConvert.SerializeObject(new HostInitContent()
            {
                ClientID = id,
                HostConfig = config
            })
        };

        public static WNMNHostMessage UpdateLocation(ShadowNoelLocation hostLocation, Dictionary<int, ShadowNoelLocation> peerLocations) => new()
        {
            Type = WNMNHostMessageType.UpdateLocation,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<ShadowNoelLocation>()
            {
                HostContent = hostLocation
            })
        };

        public static WNMNHostMessage NotifyChangeMapBefore() => new()
        {
            Type = WNMNHostMessageType.NotifyChangeMapBefore
        };

        public static WNMNHostMessage NotifyChangeMapAfter(string hostMpKey, Dictionary<int, string> peerMpKeys) => new()
        {
            Type = WNMNHostMessageType.NotifyChangeMapAfter,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<string>()
            {
                HostContent = hostMpKey
            })
        };

        public static WNMNHostMessage NotifyStateChange(PR.STATE STATE, Dictionary<int, PR.STATE> peerSTATEs) => new()
        {
            Type = WNMNHostMessageType.NotifyStateChange,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<PR.STATE>()
            {
                HostContent = STATE
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
        UpdateLocation,
        NotifyChangeMapBefore,
        NotifyChangeMapAfter,
        NotifyStateChange
    }
}
