using UnityEngine;
using WeNeedMoreNoels.CSNetworking;

namespace WeNeedMoreNoels
{
    public static class WNMNTools
    {
        static WNMNHost host;
        static WNMNClient client;

        public static void InitNetworking(NetWorkType type)
        {
            DB.networkType = type;
            if (GameObject.Find("NetworkSource") != null)
            {
                Plugin.Logger.LogWarning("NetworkSource created");
                return;
            }
            GameObject gameObject = new("NetworkSource");
            switch (DB.networkType)
            {
                case NetWorkType.Host:
                    gameObject.AddComponent<WNMNHost>();
                    break;
                case NetWorkType.Client:
                    gameObject.AddComponent<WNMNClient>();
                    break;
            }
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
    }

    public enum NetWorkType
    {
        Host,
        Client
    }
}
