using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using WeNeedMoreNoels.CSNetworking;
using WeNeedMoreNoels.Networking;

namespace WeNeedMoreNoels
{
    public static class WNMNTools
    {
        static WNMNHost host;
        static WNMNClient client;

        public static WNMNPeer peer;

        public static NetWorkType Type;
        public static int LocalID;
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

        public class NetworkConfig
        {
            public NetWorkType Type;

            public int port;

            public string ip;

            public NoelType NoelType;

            public string nickName;
        }
    }

    public enum NetWorkType
    {
        Host,
        Client
    }

    public enum NoelType
    {
        Normal,
        Inverse
    }
}
