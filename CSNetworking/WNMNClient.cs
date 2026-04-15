using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;

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
        }

        private void Update()
        {
            client.PollEvents();
            if (hostPeer is null)
            {
                return;
            }
            SendLocation(peerID, hostPeer);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            hostPeer = peer;
            //host
            ShadowNoelExtensions.GenerateShadowNoel(0);
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
            InitClient(message);
            DebugLocation(message);
            UpdateLocation(message);
        }

        private void InitClient(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.Init)
            {
                return;
            }
            peerID = int.Parse(message.Content);
        }

        private void SendLocation(int id, NetPeer peer)
        {
            NetDataWriter writer = new();
            System.Numerics.Vector2 location = NetworkConnectionTools.GetSendLocation();
            WNMNClientMessage message = WNMNClientMessage.ReportLocation(id, location);
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
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
            UpdateLocationContent content = JsonConvert.DeserializeObject<UpdateLocationContent>(message.Content);
            Plugin.Logger.LogInfo($"Host location: " + content.HostPosition);
        }

        private void UpdateLocation(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.UpdateLocation)
            {
                return;
            }
            UpdateLocationContent content = JsonConvert.DeserializeObject<UpdateLocationContent>(message.Content);
            //host
            NetworkConnectionTools.UpdateShadowLocation(0, content.HostPosition);
            //peers
            if (content.PeerPositions is null)
            {
                return;
            }
            foreach (KeyValuePair<int, System.Numerics.Vector2> pair in content.PeerPositions)
            {
                NetworkConnectionTools.UpdateShadowLocation(pair.Key, pair.Value);
            }
        }

        private void OnDestroy()
        {
            client.DisconnectAll();
            client.Stop();
        }
    }
}
