using LiteNetLib;
using nel;
using System.Collections.Generic;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;
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

        public static ShadowNoelInfo GetSendInfo()
        {
            PRNoel noel = DB.MainPR;
            noel.getPosition(out float x, out float y);
            PrNoelAnimator animator = (PrNoelAnimator)noel.Anm;
            return new()
            {
                Position = new(x, y),
                IsCrouch = noel.is_crouch,
                Pose = animator.pose_title,
                AIM = (AIM)animator.pose_aim,
                HP = noel.hp,
                MP = noel.mp
            };
        }

        public static string GetSendMpKey()
        {
            PRNoel noel = DB.MainPR;
            return noel.Mp.key;
        }

        public static void UpdateShadowInfo(int id, ShadowNoelInfo info)
        {
            if (!DB.noelEnables.ContainsKey(id) || !DB.noelEnables[id])
            {
                return;
            }
            ShadowNoel noel = DB.noelDics[id];
            System.Numerics.Vector2 pos = info.Position;
            if (info.IsCrouch)
            {
                pos.Y -= 0.5f;
            }
            ShadowNoelExtensions.MoveShadowNoel(noel, pos);
            ShadowNoelExtensions.SetPoseShadowNoel(noel, info.Pose, info.AIM);
            ShadowNoelExtensions.SetHPMP(noel, info.HP, info.MP);
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

        public static void DisconnectClient(int id)
        {
            if (DB.noelDics.ContainsKey(id))
            {
                ShadowNoel noel = DB.noelDics[id];
                Object.DestroyImmediate(noel);
                DB.noelDics.Remove(id);
            }
            if (DB.noelConfigs.ContainsKey(id))
            {
                DB.noelConfigs.Remove(id);
            }
            if (DB.noelNicknames.ContainsKey(id))
            {
                DB.noelNicknames.Remove(id);
            }
            if (DB.noelEnables.ContainsKey(id))
            {
                DB.noelEnables.Remove(id);
            }
            if (DB.noelMpKeys.ContainsKey(id))
            {
            DB.noelMpKeys.Remove(id);
            }
        }
    }
}
