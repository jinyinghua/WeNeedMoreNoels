using LiteNetLib;
using LiteNetLib.Utils;
using nel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHost : MonoBehaviour
    {
        NetManager host;

        NetManager transferHost;

        public int maxPlayerCount = 2;

        private void Awake()
        {
            EventBasedNetListener listener = new();
            host = new(listener);
            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
            EventBasedNetListener transferListener = new();
            transferHost = new(transferListener);
            transferListener.ConnectionRequestEvent += TransferListener_ConnectionRequestEvent;
            transferListener.PeerConnectedEvent += TransferListener_PeerConnectedEvent;
            NetworkConnectionTools.host = this;
        }

        private void Update()
        {
            host.PollEvents();
            transferHost.PollEvents();
            if (host is null || host.Count() == 0)
            {
                return;
            }
            HostSendLocation();
        }

        private void HostSendLocation()
        {
            ShadowNoelInfo location = NetworkConnectionTools.GetSendInfo();
            WNMNHostMessage message = WNMNHostMessage.UpdateInfo(location, null);
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
            transferHost.Start(port + 1);
            Plugin.Logger.LogInfo($"WNMN Sync transfer host started, port:{port + 1}");
        }

        private void TransferListener_ConnectionRequestEvent(ConnectionRequest request)
        {
            if (host.ConnectedPeersCount < maxPlayerCount /* max connections */)
                request.AcceptIfKey(DB.TRANSFER_ACCESS_KEY);
            else
                request.Reject();
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            if (host.ConnectedPeersCount < maxPlayerCount /* max connections */)
                request.AcceptIfKey(DB.CONNECTION_ACCESS_KEY);
            else
                request.Reject();
        }

        private void TransferListener_PeerConnectedEvent(NetPeer peer)
        {
            Plugin.Logger.LogInfo($"WNMNTransfer host got connection: {peer.EndPoint}");
            NetDataWriter writer = new();
            writer.Put(DB.SyncSaveContentBuffer);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            Plugin.Logger.LogInfo("WNMNTransfer host transfered map data");
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            Plugin.Logger.LogInfo($"WNMNMain host got connection: {peer.EndPoint}");
            NetDataWriter writer = new();
            int id = NetworkConnectionTools.Unique_ID;
            WNMNHostMessage message = WNMNHostMessage.Init(id, DB.InitConfig);
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            NetworkConnectionTools.NetPeerDic.Add(id, peer);
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
            InitHost(message);
            DebugLocationClient(message);
            UpdateLocation(message);
            UpdateChangeMapBefore(message);
            UpdateChangeMapAfter(message);
            UpdateNotifyStateChange(message);
            UpdateNotifyNoelDamage(message);
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            NetDataReader reader = disconnectInfo.AdditionalData;
            int id = reader.GetInt();
            NetworkConnectionTools.DisconnectClient(id);
        }

        private void InitHost(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.Init)
            {
                return;
            }
            WNMNTools.NetworkConfig content = JsonConvert.DeserializeObject<WNMNTools.NetworkConfig>(message.Content);
            int id = message.PeerID;
            DB.noelConfigs.Add(id, content);
            ShadowNoel noel = ShadowNoelExtensions.GenerateShadowNoel(id);
            noel.OnNoelDamage += HostSendNotifyNoelDamage;
        }

        private void DebugLocationClient(WNMNClientMessage message)
        {
            if (!DB.ShowLocationDebug)
            {
                return;
            }
            if (message.Type != WNMNClientMessageType.ReportInfo)
            {
                return;
            }
            Plugin.Logger.LogInfo($"Client#{message.PeerID} location: " + message.Content);
        }

        private void UpdateLocation(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.ReportInfo)
            {
                return;
            }
            ShadowNoelInfo location = JsonConvert.DeserializeObject<ShadowNoelInfo>(message.Content);
            NetworkConnectionTools.UpdateShadowInfo(message.PeerID, location);
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

        private void HostSendNotifyNoelDamage(int id, ShadowNoelDamage Atk)
        {
            NetDataWriter writer = new();
            WNMNHostMessage message;
            if (id == 0)
            {
                message = WNMNHostMessage.NotifyNoelDamage(Atk, null);
            }
            else
            {
                Dictionary<int, ShadowNoelDamage> noelDmgs = [];
                noelDmgs.Add(id, Atk);
                message = WNMNHostMessage.NotifyNoelDamage(null, noelDmgs);
            }
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

        private void UpdateNotifyNoelDamage(WNMNClientMessage message)
        {
            if (message.Type != WNMNClientMessageType.NotifyNoelDamage)
            {
                return;
            }
            ShadowNoelExtensions.DamageNoel(message.PeerID, JsonConvert.DeserializeObject<ShadowNoelDamage>(message.Content));
        }

        private void OnDestroy()
        {
            host.DisconnectAll();
            host.Stop();
        }
    }
}
