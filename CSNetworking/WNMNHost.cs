using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using UnityEngine;

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
                PeerInfos = [..DB.peerInfos]
            };
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            int id = reader.GetInt();
            string ip = reader.GetString();
            int port = reader.GetInt();
            ConnectPeerInfo info = new()
            {
                IP = ip,
                Port = port
            };
            DB.peerInfos.Add(id, info);
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            //TODO:断连
        }

        private void OnDestroy()
        {
            host.DisconnectAll();
            host.Stop();
            DB.noelIns.Clear();
        }
    }
}
