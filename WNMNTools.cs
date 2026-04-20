using UnityEngine;
using WeNeedMoreNoels.CSNetworking;

namespace WeNeedMoreNoels
{
    public static class WNMNTools
    {
        static WNMNHost host;
        static WNMNClient client;

        public static NetWorkType Type;
        public static int LocalID;

        public static void InitNetworking(NetworkConfig config)
        {
            InitNetworking(config.Type, out host, out client);
            switch (config.Type)
            {
                case NetWorkType.Host:
                    RunHost(config.port);
                    Type = NetWorkType.Host;
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
                    NetworkConnectionTools.IsHost = true;
                    client = null;
                    break;
                case NetWorkType.Client:
                    client = gameObject.AddComponent<WNMNClient>();
                    NetworkConnectionTools.IsHost = false;
                    host = null;
                    break;
                default:
                    host = null;
                    client = null;
                    break;
            }
            NetworkConnectionTools.Inited = true;
        }

        public static void RunHost(int port = 4721)
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
