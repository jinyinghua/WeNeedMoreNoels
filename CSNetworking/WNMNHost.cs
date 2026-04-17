using LiteNetLib;
using LiteNetLib.Utils;
using nel;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using WeNeedMoreNoels.HostMessages;
using XX;
using static m2d.M2MoverPr;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHost : MonoBehaviour
    {
        NetManager host;

        public int maxPlayerCount = 2;

        private void Awake()
        {
            EventBasedNetListener listener = new();
            host = new(listener);
            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            NetworkConnectionTools.host = this;
        }

        private void Update()
        {
            host.PollEvents();
            if (host is null || host.Count() == 0)
            {
                return;
            }
            HostSendLocation();
        }

        private void HostSendLocation()
        {
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
            WNMNHostMessage message = WNMNHostMessage.UpdateLocation(location, null);
            string json = JsonConvert.SerializeObject(message);
            NetDataWriter writer = new();
            writer.Put(json);
            host.SendToAll(writer, DeliveryMethod.Unreliable);
        }

        public void StartHost(int port = 4721)
        {
            if (host is null)
            {
                Plugin.Logger.LogWarning("WNMNHost not initialized!");
                return;
            }
            host.Start(port);
            Plugin.Logger.LogInfo($"WNMNHost started, port:{port}");
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            if (host.ConnectedPeersCount < maxPlayerCount /* max connections */)
                request.AcceptIfKey(DB.CONNECTION_ACCESS_KEY);
            else
                request.Reject();
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            Plugin.Logger.LogInfo($"We got connection: {peer.EndPoint}");
            NetDataWriter writer = new();
            int id = NetworkConnectionTools.Unique_ID;
            WNMNHostMessage message = WNMNHostMessage.Init(id);
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            NetworkConnectionTools.NetPeerDic.Add(id, peer);
            ShadowNoelExtensions.GenerateShadowNoel(id);
            NetworkConnectionTools.Connected = true;
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string json = reader.GetString();
            WNMNClientMessage message = JsonConvert.DeserializeObject<WNMNClientMessage>(json);
            if (DB.ShowReceiveDebug)
            {
                Plugin.Logger.LogInfo($"Message from client: {message}");
            }
            reader.Recycle();
            DebugLocationClient(message);
            UpdateLocation(message);
            UpdateChangeMapBefore(message);
            UpdateChangeMapAfter(message);
            UpdateNotifyStateChange(message);
        }

        private void DebugLocationClient(WNMNClientMessage message)
        {
            if (!DB.ShowLocationDebug)
            {
                return;
            }
            if (message.Type != WNMNClientMessageType.ReportLocation)
            {
                return;
            }
            Plugin.Logger.LogInfo($"Client#{message.PeerID} location: " + message.Content);
        }

        private void UpdateLocation(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.ReportLocation)
            {
                return;
            }
            ShadowNoelLocation location = JsonConvert.DeserializeObject<ShadowNoelLocation>(message.Content);
            NetworkConnectionTools.UpdateShadowLocation(message.PeerID, location.Position, location.IsCrouch);
            NetworkConnectionTools.UpdateShadowPose(message.PeerID, location.Pose, location.AIM);
        }

        public void HostSendNotifyChangeMapBefore()
        {
            NetDataWriter writer = new();
            WNMNHostMessage message = WNMNHostMessage.NotifyChangeMapBefore();
            writer.Put(JsonConvert.SerializeObject(message));
            host.SendToAll(writer, DeliveryMethod.Unreliable);
        }

        public void HostSendNotifyChangeMapAfter(string key)
        {
            NetDataWriter writer = new();
            WNMNHostMessage message = WNMNHostMessage.NotifyChangeMapAfter(key, null);
            writer.Put(JsonConvert.SerializeObject(message));
            host.SendToAll(writer, DeliveryMethod.Unreliable);
        }

        private void UpdateChangeMapBefore(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.NotifyChangeMapBefore)
            {
                return;
            }
            ShadowNoelExtensions.DisableAllShadowNoels();
        }

        private void UpdateChangeMapAfter(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.NotifyChangeMapAfter)
            {
                return;
            }
            ShadowNoelExtensions.UpdateShadowNoelMpKey(message.PeerID, message.Content);
            ShadowNoelExtensions.DetectShadowNoelInCurrentMap();
        }

        public void HostSendNotifyStateChange(PR.STATE STATE)
        {
            NetDataWriter writer = new();
            WNMNHostMessage message = WNMNHostMessage.NotifyStateChange(STATE, null);
            writer.Put(JsonConvert.SerializeObject(message));
            host.SendToAll(writer, DeliveryMethod.Unreliable);
        }

        private void UpdateNotifyStateChange(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.NotifyStateChange)
            {
                return;
            }
            ShadowNoelExtensions.UpdateShadowNoelState(message.PeerID, (PR.STATE)int.Parse(message.Content));
        }

        private void OnDestroy()
        {
            host.DisconnectAll();
            host.Stop();
        }
    }
}
