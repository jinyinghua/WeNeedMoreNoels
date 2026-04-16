using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;
using XX;

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
            if (peerID == 0)
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
            WNMNClientMessage message = WNMNClientMessage.ReportLocation(id, location);
            writer.Put(JsonConvert.SerializeObject(message));
            peer.Send(writer, DeliveryMethod.Unreliable);
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
            HostUpdateContent<ShadowNoelLocation> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelLocation>>(message.Content);
            Plugin.Logger.LogInfo($"Host location: " + content.HostContent.ToString());
        }

        private void UpdateLocation(WNMNHostMessage message)
        {
            if (message.Type != WNMNHostMessageType.UpdateLocation)
            {
                return;
            }
            HostUpdateContent<ShadowNoelLocation> content = JsonConvert.DeserializeObject<HostUpdateContent<ShadowNoelLocation>>(message.Content);
            //host
            NetworkConnectionTools.UpdateShadowLocation(0, content.HostContent.Position, content.HostContent.IsCrouch);
            NetworkConnectionTools.UpdateShadowPose(0, content.HostContent.Pose, content.HostContent.AIM);
            //peers
            if (content.PeerContents is null)
            {
                return;
            }
            foreach (KeyValuePair<int, ShadowNoelLocation> pair in content.PeerContents)
            {
                NetworkConnectionTools.UpdateShadowLocation(pair.Key, pair.Value.Position, pair.Value.IsCrouch);
                NetworkConnectionTools.UpdateShadowPose(pair.Key, pair.Value.Pose, pair.Value.AIM);
            }
        }

        private void OnDestroy()
        {
            client.DisconnectAll();
            client.Stop();
        }
    }
}
