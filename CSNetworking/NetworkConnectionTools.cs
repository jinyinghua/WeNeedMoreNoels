using LiteNetLib;
using nel;
using System.Collections.Generic;
using System.Numerics;
using XX;

namespace WeNeedMoreNoels.CSNetworking
{
    public static class NetworkConnectionTools
    {
        public static bool IsHost;

        public static bool Inited;

        public static bool Connected;

        public static WNMNClient client;

        public static WNMNHost host;

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

        public static bool GetSendCrouch()
        {
            PRNoel noel = DB.MainPR;
            return noel.is_crouch;
        }

        public static string GetSendPose()
        {
            PRNoel noel = DB.MainPR;
            PrNoelAnimator animator = (PrNoelAnimator)noel.Anm;
            return animator.pose_title;
        }

        public static AIM GetSendAIM()
        {
            PRNoel noel = DB.MainPR;
            PrNoelAnimator animator = (PrNoelAnimator)noel.Anm;
            return (AIM)animator.pose_aim;
        }

        public static string GetSendMpKey()
        {
            PRNoel noel = DB.MainPR;
            return noel.Mp.key;
        }

        public static void UpdateShadowLocation(int id, Vector2 pos, bool isCrouch)
        {
            if (!DB.noelEnables[id])
            {
                return;
            }
            ShadowNoel noel = DB.noelDics[id];
            if (isCrouch)
            {
                pos.Y -= 0.5f;
            }
            ShadowNoelExtensions.MoveShadowNoel(noel, pos);
        }

        public static void UpdateShadowPose(int id, string pose, AIM aim)
        {
            if (!DB.noelEnables[id])
            {
                return;
            }
            ShadowNoel noel = DB.noelDics[id];
            ShadowNoelExtensions.SetPoseShadowNoel(noel, pose, aim);
        }

        public static void NotifyChangeMapBefore()
        {
            if (!Inited | !Connected)
            {
                return;
            }
            if (IsHost)
            {
                host.HostSendNotifyChangeMapBefore();
            }
            else
            {
                client.SendNotifyChangeMapBefore();
            }
        }

        public static void NotifyChangeMapAfter(string key)
        {
            if (!Inited | !Connected)
            {
                return;
            }
            if (IsHost)
            {
                host.HostSendNotifyChangeMapAfter(key);
            }
            else
            {
                client.SendNotifyChangeMapAfter(key);
            }
        }

        public static void NotifyStateChange(PR.STATE STATE)
        {
            if (!Inited | !Connected)
            {
                return;
            }
            if (IsHost)
            {
                host.HostSendNotifyStateChange(STATE);
            }
            else
            {
                client.SendNotifyStateChange(STATE);
            }
        }
    }
}
