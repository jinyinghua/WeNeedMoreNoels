using nel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHostMessage
    {
        public WNMNHostMessageType Type;

        public string Content;

        public static WNMNHostMessage Init(int id, WNMNTools.NetworkConfig config, Dictionary<int, WNMNTools.NetworkConfig> peerConfigs) => new()
        {
            Type = WNMNHostMessageType.Init,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<HostInitContent>()
            {
                HostContent = new()
                {
                    ClientID = id,
                    HostConfig = config
                },
                PeerContents = [.. peerConfigs.Select(x => new KeyValuePair<int, HostInitContent>(x.Key, new()
                {
                    ClientID = x.Key,
                    HostConfig = x.Value
                }))]
            })
        };

        public static WNMNHostMessage InitClient(int id, WNMNTools.NetworkConfig config) => new()
        {
            Type = WNMNHostMessageType.InitOtherClient,
            Content = JsonConvert.SerializeObject(new HostInitContent()
            {
                ClientID = id,
                HostConfig = config
            })
        };

        public static WNMNHostMessage DisconnectOtherClient(int id) => new()
        {
            Type = WNMNHostMessageType.DisconnectOtherClient,
            Content = id.ToString()
        };

        public static WNMNHostMessage UpdateInfo(ShadowNoelInfo hostInfo, Dictionary<int, ShadowNoelInfo> peerInfos) => new()
        {
            Type = WNMNHostMessageType.UpdateInfo,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<ShadowNoelInfo>()
            {
                HostContent = hostInfo,
                PeerContents = [.. peerInfos]
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

        public static WNMNHostMessage NotifyNoelDamage(ShadowNoelDamage val, Dictionary<int, ShadowNoelDamage> peerVals) => new()
        {
            Type = WNMNHostMessageType.NotifyNoelDamage,
            Content = JsonConvert.SerializeObject(new HostUpdateContent<ShadowNoelDamage>()
            {
                HostContent = val,
                PeerContents = [.. peerVals]
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
        InitOtherClient,
        DisconnectOtherClient,
        UpdateInfo,
        NotifyChangeMapBefore,
        NotifyChangeMapAfter,
        NotifyStateChange,
        NotifyNoelDamage
    }
}
