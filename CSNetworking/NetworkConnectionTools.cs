using LiteNetLib;
using nel;
using System.Collections.Generic;
using System.Numerics;

namespace WeNeedMoreNoels.CSNetworking
{
    public static class NetworkConnectionTools
    {
        public static int Unique_ID
        {
            get
            {
                _uniqueID++;
                return _uniqueID;
            }
        }

        public static Dictionary<int, NetPeer> NetPeerDic = new();

        static int _uniqueID = 0;

        public static Vector2 GetSendLocation()
        {
            PRNoel noel = DB.MainPR;
            noel.getPosition(out float x, out float y);
            return new(x, y);
        }

        public static void UpdateShadowLocation(int id, Vector2 pos)
        {
            ShadowNoel noel = DB.noelDics[id];
            ShadowNoelExtensions.MoveShadowNoel(noel, pos);
        }
    }
}
