using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;
using System.IO;
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
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
        }

        private void Update()
        {
            if (DB.MainPR == null || DB.MainPR.Mp == null)
            {
                return;
            }
            WNMNTools.SendUpdateToAllPeers(WNMNTools.LocalID);
            localPeer?.PollEvents();
            WNMNTools.UpdateAllNoels();
            WNMNTools.SetAllNickNameBgs();
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            Plugin.Logger.LogInfo($"peer got request: {request.RemoteEndPoint}");
            request.AcceptIfKey(DB.P2P_ACCESS_KEY);
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte[] receivedData = new byte[reader.UserDataSize];
            reader.GetBytes(receivedData, reader.UserDataSize);
            using MemoryStream stream = new();
            stream.Write(receivedData, 0, receivedData.Length);
            stream.Seek(0, SeekOrigin.Begin);
            WNMNPeerMessage message = Serializer.Deserialize<WNMNPeerMessage>(stream);
            foreach (PeerReceiveMessageBase receive in ReceiveMessageManager.GetAllReceives(message))
            {
                if (receive.CheckMessage(message))
                {
                    receive.ReceiveMessage(message);
                }
            }
        }

        public void SendToAll(byte[] content, DeliveryMethod delivery)
        {
            NetDataWriter writer = new();
            writer.Put(content);
            localPeer.SendToAll(writer, delivery);
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
