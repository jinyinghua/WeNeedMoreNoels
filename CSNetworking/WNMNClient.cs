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
            client.Connect(ip, port, DB.CONNECTION_ACCESS_KEY);
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string json = reader.GetString();
            WNMNHostMessage message = JsonConvert.DeserializeObject<WNMNHostMessage>(json);
            reader.Recycle();
            peerID = message.InitID;
            WNMNTools.LocalID = message.InitID;
            PartyManager.Party party = PartyManager.InitNewParty(message.InitID);
            DB.partyInfos.Add(message.InitID, party);
            DB.LocalNoelParty = message.InitID;
            WNMNTools.ConnectOtherPeer(message.PeerInfos);
            WNMNTools.SendInitToAllPeers(message.InitID);
            WNMNTools.GenerateAllNoels(message.PeerConfigs);
            NetDataWriter writer = new();
            WNMNClientMessage message1 = new()
            {
                ID = peerID,
                IP = WNMNTools.GetLocalIP(),
                Port = WNMNTools.peer.GetPeerPort(),
                NickName = DB.InitConfig.nickName,
                NoelType = DB.InitConfig.NoelType
            };
            writer.Put(JsonConvert.SerializeObject(message1));
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
