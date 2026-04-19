using LiteNetLib;
using LiteNetLib.Utils;
using m2d;
using nel;
using nel.gm;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;
using XX;

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
            SendLocation(peerID, hostPeer);
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
            DB.noelConfigs.Add(0, content.HostConfig);
            ShadowNoelExtensions.GenerateShadowNoel(0);
            WNMNClientMessage initMessage = WNMNClientMessage.Init(peerID, DB.InitConfig);
            NetDataWriter writer = new();
            writer.Put(JsonConvert.SerializeObject(initMessage));
            hostPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void SendLocation(int id, NetPeer peer)
        {
            NetDataWriter writer = new();
            System.Numerics.Vector2 position = NetworkConnectionTools.GetSendLocation();
            bool crouch = NetworkConnectionTools.GetSendCrouch();
            string pose = NetworkConnectionTools.GetSendPose();
            AIM aim = NetworkConnectionTools.GetSendAIM();
            ShadowNoelLocation location = new()
            {
                Position = position,
                IsCrouch = crouch,
                Pose = pose,
                AIM = aim
            };
            WNMNClientMessage message = WNMNClientMessage.ReportLocation(id, location);
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.Unreliable);
        }

        private void DebugLocation(WNMNHostMessage message)
        {
            if (!DB.ShowLocationDebug)
            {
                return;
            }
            if (message.Type != WNMNHostMessageType.UpdateLocation)
            {
                return;
            }
            HostUpdateContent<ShadowNoelLocation> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelLocation>>(message.Content);
            Plugin.Logger.LogInfo($"Host location: " + content.HostContent.ToString());
        }

        private void UpdateLocation(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.UpdateLocation)
            {
                return;
            }
            HostUpdateContent<ShadowNoelLocation> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelLocation>>(message.Content);
            //host
            NetworkConnectionTools.UpdateShadowLocation(0, content.HostContent.Position, content.HostContent.IsCrouch);
            NetworkConnectionTools.UpdateShadowPose(0, content.HostContent.Pose, content.HostContent.AIM);
            //peers
            if (content.PeerContents is null)
            {
                return;
            }
            foreach (KeyValuePair<int, ShadowNoelLocation> pair in content.PeerContents)
            {
                NetworkConnectionTools.UpdateShadowLocation(pair.Key, pair.Value.Position, pair.Value.IsCrouch);
                NetworkConnectionTools.UpdateShadowPose(pair.Key, pair.Value.Pose, pair.Value.AIM);
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
