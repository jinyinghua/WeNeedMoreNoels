using LiteNetLib;
using nel;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking
{
    public class WNMNPeer : MonoBehaviour
    {
        NetManager localPeer;

        private void Awake()
        {
            EventBasedNetListener listener = new();
            localPeer = new(listener);
            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
        }

        private void Update()
        {
            localPeer?.PollEvents();
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            Plugin.Logger.LogInfo($"peer got request: {request.RemoteEndPoint}");
            request.AcceptIfKey(DB.P2P_ACCESS_KEY);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {

        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte[] receivedData = new byte[reader.UserDataSize];
            reader.GetBytes(receivedData, reader.UserDataSize);
            WNMNPeerMessage message = WNMNPeerMessage.Parser.ParseFrom(receivedData);
            foreach (PeerReceiveMessageBase receive in ReceiveMessageManager.GetAllReceives(message))
            {
                if (!receive.CheckMessage(message))
                {
                    continue;
                }
                receive.ReceiveMessage(message);
            }
        }

        public void ConnectPeer(string ip, int port)
        {
            localPeer.Connect(ip, port, DB.P2P_ACCESS_KEY);
            Plugin.Logger.LogInfo($"peer connect {ip}:{port}");
        }

        public int StartPeer()
        {
            localPeer.Start();
            return GetPeerPort();
        }

        public int GetPeerPort()
        {
            return localPeer.LocalPort;
        }
    }
}
