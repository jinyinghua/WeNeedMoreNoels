using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNHost : MonoBehaviour
    {
        NetManager host;

        NetManager transferHost;

        public int maxPlayerCount = 5;

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
            WNMNTools.LocalID = 0;
            PartyManager.Party party = PartyManager.InitNewParty(0);
            DB.partyInfos.Add(0, party);
        }

        private void Update()
        {
            host?.PollEvents();
            transferHost?.PollEvents();
        }

        public int GetPort()
        {
            return host.LocalPort;
        }

        public void StartHost(int port = 47210)
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
            Plugin.Logger.LogInfo($"Transfer host got request: {request.RemoteEndPoint}");
            if (host.ConnectedPeersCount < maxPlayerCount /* max connections */)
                request.AcceptIfKey(DB.TRANSFER_ACCESS_KEY);
            else
                request.Reject();
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            Plugin.Logger.LogInfo($"Host got request: {request.RemoteEndPoint}");
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
            int id = WNMNTools.Unique_ID;
            WNMNHostMessage message = new()
            {
                InitID = id,
                PeerInfos = [..DB.peerInfos],
                PeerConfigs = [.. DB.peerConfigs],
                PeerParties = [.. DB.partyInfos]
            };
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string json = reader.GetString();
            WNMNClientMessage message = JsonConvert.DeserializeObject<WNMNClientMessage>(json);
            ConnectPeerInfo info = new()
            {
                IP = message.IP,
                Port = message.Port
            };
            DB.peerInfos.Add(message.ID, info);
            ClientConfig config = new()
            {
                Nickname = message.NickName,
                NoelType = message.NoelType,
                NoelColor = message.NoelColor
            };
            DB.peerConfigs.Add(message.ID, config);
            DB.partyInfos.Add(message.ID, message.Party);
            WNMNHostMessage message1 = new()
            {
                InitOther = true,
                ExcludeID = message.ID,
                PeerParties = [new(message.ID, message.Party)]
            };
            NetDataWriter writer = new();
            writer.Put(JsonConvert.SerializeObject(message1));
            host.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            WNMNTools.SetAllNickNameBgs();
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            int id = disconnectInfo.AdditionalData.GetInt();
            WNMNTools.DisconnectClient(id);
        }

        private void OnDestroy()
        {
            host.DisconnectAll();
            host.Stop();
            DB.InitConfig = null;
            DB.noelIns.Clear();
            DB.partyInfos.Clear();
            DB.peerInfos.Clear();
            DB.peerConfigs.Clear();
        }
    }
}
