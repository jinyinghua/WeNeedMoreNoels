using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Linq;
using System.Numerics;
using UnityEngine;

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
            WNMNHostMessage message = WNMNHostMessage.UpdateLocation(position, null);
            string json = JsonConvert.SerializeObject(message);
            NetDataWriter writer = new();
            writer.Put(json);
            host.SendToAll(writer, DeliveryMethod.ReliableOrdered);
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
            NetworkConnectionTools.UpdateShadowLocation(message.PeerID, JsonConvert.DeserializeObject<System.Numerics.Vector2>(message.Content));
        }

        private void OnDestroy()
        {
            host.DisconnectAll();
            host.Stop();
        }
    }
}
