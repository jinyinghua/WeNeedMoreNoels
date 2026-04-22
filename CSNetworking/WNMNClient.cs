using LiteNetLib;
using LiteNetLib.Utils;
using nel;
using Newtonsoft.Json;
using UnityEngine;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNClient : MonoBehaviour
    {
        NetManager client;

        NetPeer hostPeer;

        public int peerID;

        private void Awake()
        {
            EventBasedNetListener listener = new();
            client = new(listener);
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
        }

        private void Update()
        {
            client?.PollEvents();
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            hostPeer = peer;
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
            reader.Recycle();
            peerID = message.InitID;
            WNMNTools.ConnectOtherPeer(message.PeerInfos);
            NetDataWriter writer = new();
            writer.Put(message.InitID);
            writer.Put(WNMNTools.GetLocalIP());
            writer.Put(WNMNTools.peer.GetPeerPort());
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            DB.WNMNHostClosed = true;
            ((NelM2DBase)DB.MainPR.M2D).quitGame("SceneTitle");
        }

        private void OnDestroy()
        {
            NetDataWriter writer = new();
            writer.Put(peerID);
            client.DisconnectPeer(hostPeer, writer);
            client.Stop();
            DB.noelIns.Clear();
        }
    }
}
