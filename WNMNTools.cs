using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using WeNeedMoreNoels.CSNetworking;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.Networking;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels
{
    public static class WNMNTools
    {
        static WNMNHost host;
        static WNMNClient client;

        public static WNMNPeer peer;

        public static NetWorkType Type;
        public static int LocalID = -1;
        public static bool Inited;

        static int _unique_id;
        public static int Unique_ID
        {
            get
            {
                _unique_id++;
                return _unique_id;
            }
        }

        public static void InitNetworking(NetworkConfig config)
        {
            InitNetworking(config.Type, out host, out client);
            GameObject gameObject = new("NetworkPeer");
            peer = gameObject.AddComponent<WNMNPeer>();
            int port = peer.StartPeer();
            switch (config.Type)
            {
                case NetWorkType.Host:
                    RunHost(config.port);
                    Type = NetWorkType.Host;
                    DB.peerInfos.Add(0, new()
                    {
                        IP = GetLocalIP(),
                        Port = port
                    });
                    DB.peerConfigs.Add(0, new()
                    {
                        Nickname = config.nickName,
                        NoelType = config.NoelType,
                        NoelColor = config.NoelColor
                    });
                    break;
                case NetWorkType.Client:
                    ConnectHost(config.ip, config.port);
                    Type = NetWorkType.Client;
                    break;
            }
            DB.Nickname = config.nickName;
        }

        public static void InitNetworking(NetWorkType type, out WNMNHost host, out WNMNClient client)
        {
            DB.networkType = type;
            if (GameObject.Find("NetworkSource") != null)
            {
                Plugin.Logger.LogWarning("NetworkSource created");
                host = null;
                client = null;
                return;
            }
            GameObject gameObject = new("NetworkSource");
            switch (DB.networkType)
            {
                case NetWorkType.Host:
                    host = gameObject.AddComponent<WNMNHost>();
                    Type = NetWorkType.Host;
                    client = null;
                    break;
                case NetWorkType.Client:
                    client = gameObject.AddComponent<WNMNClient>();
                    Type = NetWorkType.Client;
                    host = null;
                    break;
                default:
                    host = null;
                    client = null;
                    break;
            }
            Inited = true;
        }

        public static string GetLocalIP()
        {
            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530); // 不会真正发送数据
            IPEndPoint endPoint = (IPEndPoint)socket.LocalEndPoint;
            return endPoint.Address.ToString();
        }

        public static void ConnectOtherPeer(List<KeyValuePair<int, ConnectPeerInfo>> peerList)
        {
            int id = Type == NetWorkType.Host ? 0 : client.peerID;
            foreach (var pair in peerList)
            {
                if (pair.Key != id)
                {
                    peer.ConnectPeer(pair.Value.IP, pair.Value.Port);
                }
            }
        }

        public static void RunHost(int port = 47210)
        {
            if (host == null)
            {
                Plugin.Logger.LogWarning("NetworkSource not initialized");
                return;
            }
            host.StartHost(port);
        }

        public static void ConnectHost(string ip = "localhost", int port = 4721)
        {
            if (client == null)
            {
                Plugin.Logger.LogWarning("NetworkSource not initialized");
                return;
            }
            client.ConnectHost(ip, port);
        }

        public static void UpdateNoel(int id, UpdateNoelInfo info)
        {
            DB.noelIns[id].NoelInfo = info;
        }

        public static void UpdateAllNoels()
        {
            foreach (var pair in DB.noelIns)
            {
                ShadowNoelExtensions.UpdateShadowNoelInfo(pair.Key);
            }
        }

        public static void GenerateAllNoels(List<KeyValuePair<int, ClientConfig>> list)
        {
            foreach (var pair in list)
            {
                ShadowNoelExtensions.GenerateShadowNoel(pair.Value, pair.Key);
            }
        }

        public static void SendInitToAllPeers(int id)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.InitNoel,
                PeerId = id,
                InitNoelConfig = new()
                {
                    Id = id,
                    ClientConfig = DB.InitConfig
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public static void SendUpdateToAllPeers(int id)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.UpdateNoelInfo,
                PeerId = id,
                UpdateNoelInfo = ShadowNoelExtensions.GetSendInfo()
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.Unreliable);
        }

        public static void SendDamageToAllPeers(int id, NotifyNoelDamage dmg)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyNoelDamage,
                PeerId = id,
                NotifyNoelDamage = dmg
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        public static void DisconnectClient(int id)
        {
            ShadowNoelExtensions.DisableShadowNoel(id);
            DB.noelIns.Remove(id);
            DB.partyInfos.Remove(id);
            DB.peerInfos.Remove(id);
            DB.peerConfigs.Remove(id);
        }

        public static void SetAllNickNameBgs()
        {
            ShadowNoelNickname nicknameIns = DB.MainPRNickname;
            nicknameIns.SetBgColor(DB.partyInfos[DB.LocalNoelParty].Color);
            foreach (var pair in DB.noelIns)
            {
                pair.Value.NicknameIns.SetBgColor(DB.partyInfos[pair.Value.Noel.PartyID].Color);
            }
        }

        public class NetworkConfig
        {
            public NetWorkType Type;

            public int port;

            public string ip;

            public NoelType NoelType;

            public ColorNoelColor NoelColor;

            public string nickName;

            public static implicit operator ClientConfig(NetworkConfig config)
            {
                return new()
                {
                    Nickname = config.nickName,
                    NoelType = config.NoelType,
                    NoelColor = config.NoelColor
                };
            }
        }
    }

    public enum NetWorkType
    {
        Host,
        Client
    }
}
