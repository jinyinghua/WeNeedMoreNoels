using m2d;
using nel;
using ProtoBuf;
using System.IO;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels
{
    public abstract class EnemySynchronizer : MonoBehaviour
    {
        public NelEnemy Enemy;

        public int SyncID;

        public bool Alive => Enemy != null && Enemy.is_alive;

        public void Awake()
        {
            Enemy = GetComponent<NelEnemy>();
        }

        protected UpdateEnemyInfo GetEnemyInfo()
        {
            Enemy.getPosition(out float x, out float y);

            var anim = Enemy.getAnimator();
            string pose;
            if (anim is EnemyAnimatorPxl animPxl)
            {
                pose = animPxl.Anm.pose_title;
            }
            else if (anim is EnemyAnimatorSpine animSpine)
            {
                pose = animSpine.pose_title0_;
            }
            else
            {
                pose = string.Empty;
            }

            return new UpdateEnemyInfo
            {
                PositionX = x,
                PositionY = y,
                Pose = pose,
                Aim = (int)Enemy.aim,
                Hp = Enemy.hp,
                Mp = Enemy.mp,
                State = (int)Enemy.state,
                T = Enemy.t,
            };
        }

        public void DamageEnemy(int hp, int mp)
        {
            var Atk = new NelAttackInfo
            {
                attr = MGATTR.NORMAL,
                ndmg = NDMG.DEFAULT,
                hpdmg0 = hp,
                mpdmg0 = mp,
                fix_damage = true,
                parryable = false,
                shield_break_ratio = 1f,
                ignore_nodamage_time = true,
                nodamage_time = 0,
            };
            Enemy.applyDamage(Atk, true);
        }


        public static int Unique_Sync_ID
        {
            get
            {
                _unique_id++;
                return _unique_id;
            }
        }

        static int _unique_id = -1;
    }

    public class EnemySynchronizerSyncHost : EnemySynchronizer
    {
        public void Update()
        {
            SendUpdateToAllPeers();
        }

        public void SendUpdateToAllPeers()
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyEnemyUpdate,
                PeerId = WNMNTools.LocalID,
                NotifyEnemyUpdate = new()
                {
                    SyncID = SyncID,
                    Type = NotifyEnemyType.InfoUpdate,
                    Info = GetEnemyInfo()
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            WNMNTools.peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.Unreliable);
        }

        void OnDestroy()
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyEnemyUpdate,
                PeerId = WNMNTools.LocalID,
                NotifyEnemyUpdate = new()
                {
                    SyncID = SyncID,
                    Type = NotifyEnemyType.Dead
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            WNMNTools.peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.Unreliable);
        }
    }

    public class EnemySynchronizerSyncClient : EnemySynchronizer
    {
        public int PeerID;

        void Update()
        {
            Enemy.Anm.alpha = 0.4f;
        }

        public void UpdateEnemyInfo(UpdateEnemyInfo info)
        {
            if (Enemy == null || Enemy.Phy == null || Enemy.Anm == null)
            {
                return;
            }
            Enemy.setTo(info.PositionX, info.PositionY);
            Enemy.Phy.killSpeedForce(true, true, true, true, true);

            var anim = Enemy.getAnimator();
            anim.setPose(info.Pose);
            Enemy.setAim((AIM)info.Aim);

            Enemy.hp = info.Hp;
            Enemy.mp = info.Mp;
            Enemy.changeState((NelEnemy.STATE)info.State);
            Enemy.t = info.T;
        }

        public void NotifyDamage(int hp, int mp)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyEnemyUpdate,
                PeerId = WNMNTools.LocalID,
                NotifyEnemyUpdate = new()
                {
                    SyncID = SyncID,
                    Type = NotifyEnemyType.NotifyDamage,
                    Damage = new()
                    {
                        hp = hp,
                        mp = mp
                    }
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            WNMNTools.peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.Unreliable);
        }
    }
}
