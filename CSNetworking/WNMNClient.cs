using LiteNetLib;
using LiteNetLib.Utils;
using nel;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNClient : MonoBehaviour
    {
        NetManager client;

        NetPeer hostPeer;

        int peerID;

        private void Awake()
        {
            EventBasedNetListener listener = new();
            client = new(listener);
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
            NetworkConnectionTools.client = this;
        }

        private void Update()
        {
            client.PollEvents();
            if (hostPeer is null)
            {
                return;
            }
            if (peerID == 0)
            {
                return;
            }
            SendInfo(peerID, hostPeer);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            hostPeer = peer;
            //host
            NetworkConnectionTools.Connected = true;
        }

        public void ConnectHost(string ip = "localhost", int port = 4721)
        {
            client.Start();
            if (client.Connect(ip, port, DB.CONNECTION_ACCESS_KEY) == null)
            {
                Plugin.Logger.LogWarning($"Unable to connect to:{ip}:{port}");
            }
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string json = reader.GetString();
            WNMNHostMessage message = JsonConvert.DeserializeObject<WNMNHostMessage>(json);
            if (DB.ShowReceiveDebug)
            {
                Plugin.Logger.LogInfo($"Message from host: {message}");
            }
            reader.Recycle();
            InitClient(peer, message);
            DebugLocation(message);
            UpdateLocation(message);
            UpdateChangeMapBefore(message);
            UpdateChangeMapAfter(message);
            UpdateNotifyStateChange(message);
            UpdateNotifyNoelDamage(message);
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            DB.WNMNHostClosed = true;
            ((NelM2DBase)DB.MainPR.M2D).quitGame("SceneTitle");
        }

        private void InitClient(NetPeer hostPeer, WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.Init)
            {
                return;
            }
            HostInitContent content = JsonConvert.DeserializeObject<HostInitContent>(message.Content);
            peerID = content.ClientID;
            WNMNTools.LocalID = content.ClientID;
            DB.noelConfigs.Add(0, content.HostConfig);
            ShadowNoel noel = ShadowNoelExtensions.GenerateShadowNoel(0);
            noel.OnNoelDamage += SendNotifyNoelDamage;
            WNMNClientMessage initMessage = WNMNClientMessage.Init(peerID, DB.InitConfig);
            NetDataWriter writer = new();
            writer.Put(JsonConvert.SerializeObject(initMessage));
            hostPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void SendInfo(int id, NetPeer peer)
        {
            NetDataWriter writer = new();
            ShadowNoelInfo info = NetworkConnectionTools.GetSendInfo();
            WNMNClientMessage message = WNMNClientMessage.ReportInfo(id, info);
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.Unreliable);
        }

        private void DebugLocation(WNMNHostMessage message)
        {
            if (!DB.ShowLocationDebug)
            {
                return;
            }
            if (message.Type != WNMNHostMessageType.UpdateInfo)
            {
                return;
            }
            HostUpdateContent<ShadowNoelInfo> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelInfo>>(message.Content);
            Plugin.Logger.LogInfo($"Host location: " + content.HostContent.ToString());
        }

        private void UpdateLocation(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.UpdateInfo)
            {
                return;
            }
            HostUpdateContent<ShadowNoelInfo> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelInfo>>(message.Content);
            //host
            NetworkConnectionTools.UpdateShadowInfo(0, content.HostContent);
            //peers
            if (content.PeerContents is null)
            {
                return;
            }
            foreach (KeyValuePair<int, ShadowNoelInfo> pair in content.PeerContents)
            {
                NetworkConnectionTools.UpdateShadowInfo(pair.Key, pair.Value);
            }
        }

        public void SendNotifyChangeMapBefore()
        {
            NetDataWriter writer = new();
            WNMNClientMessage message = WNMNClientMessage.NotifyChangeMapBefore(peerID);
            writer.Put(JsonConvert.SerializeObject(message));
            hostPeer.Send(writer, DeliveryMethod.Unreliable);
        }

        public void SendNotifyChangeMapAfter(string key)
        {
            NetDataWriter writer = new();
            WNMNClientMessage message = WNMNClientMessage.NotifyChangeMapAfter(peerID, key);
            writer.Put(JsonConvert.SerializeObject(message));
            hostPeer.Send(writer, DeliveryMethod.Unreliable);
        }

        private void SendNotifyNoelDamage(int id, ShadowNoelDamage Atk)
        {
            NetDataWriter writer = new();
            WNMNClientMessage message = WNMNClientMessage.NotifyNoelDamage(id, Atk);
            writer.Put(JsonConvert.SerializeObject(message));
            hostPeer.Send(writer, DeliveryMethod.Unreliable);
        }

        private void UpdateChangeMapBefore(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.NotifyChangeMapBefore)
            {
                return;
            }
            ShadowNoelExtensions.DisableAllShadowNoels();
        }

        private void UpdateChangeMapAfter(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.NotifyChangeMapAfter)
            {
                return;
            }
            HostUpdateContent<string> content = JsonConvert.DeserializeObject<HostUpdateContent<string>>(message.Content);
            //host
            ShadowNoelExtensions.UpdateShadowNoelMpKey(0, content.HostContent);
            //peers
            if (content.PeerContents is not null)
            {
                foreach (KeyValuePair<int, string> pair in content.PeerContents)
                {
                    ShadowNoelExtensions.UpdateShadowNoelMpKey(pair.Key, pair.Value);
                }
            }
            ShadowNoelExtensions.DetectShadowNoelInCurrentMap();
        }

        public void SendNotifyStateChange(PR.STATE STATE)
        {
            if (hostPeer is null)
            {
                return;
            }
            NetDataWriter writer = new();
            WNMNClientMessage message = WNMNClientMessage.NotifyStateChange(peerID, STATE);
            writer.Put(JsonConvert.SerializeObject(message));
            hostPeer.Send(writer, DeliveryMethod.Unreliable);
        }

        private void UpdateNotifyStateChange(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.NotifyStateChange)
            {
                return;
            }
            HostUpdateContent<PR.STATE> content = JsonConvert.DeserializeObject<HostUpdateContent<PR.STATE>>(message.Content);
            //host
            ShadowNoelExtensions.UpdateShadowNoelState(0, content.HostContent);
            //peers
            if (content.PeerContents is not null)
            {
                foreach (KeyValuePair<int, PR.STATE> pair in content.PeerContents)
                {
                    ShadowNoelExtensions.UpdateShadowNoelState(pair.Key, pair.Value);
                }
            }
        }

        private void UpdateNotifyNoelDamage(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.NotifyNoelDamage)
            {
                return;
            }
            HostUpdateContent<ShadowNoelDamage> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelDamage>>(message.Content);
            if (content.HostContent is not null)
            {
                ShadowNoelExtensions.DamageNoel(0, content.HostContent);
            }
            else
            {
                foreach (KeyValuePair<int, ShadowNoelDamage> pair in content.PeerContents)
                {
                    ShadowNoelExtensions.DamageNoel(pair.Key, pair.Value);
                }
            }
        }

        private void OnDestroy()
        {
            NetDataWriter writer = new();
            writer.Put(peerID);
            client.DisconnectPeer(hostPeer, writer);
            client.Stop();
            NetworkConnectionTools.DisconnectClient(0);
        }
    }
}
