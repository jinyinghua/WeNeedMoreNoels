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
        public NelEnemy MoverEnemy;

        public int Id => MoverEnemy.index;

        public string Key => MoverEnemy.key;

        public void Awake()
        {
            MoverEnemy = gameObject.GetComponent<NelEnemy>();
        }

        protected UpdateEnemyInfo GetEnemyInfo()
        {
            MoverEnemy.getPosition(out float x, out float y);

            var anim = MoverEnemy.getAnimator();
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
                Key = MoverEnemy.key,
                PositionX = x,
                PositionY = y,
                Pose = pose,
                Aim = (int)MoverEnemy.aim,
                Hp = MoverEnemy.hp,
                Mp = MoverEnemy.mp,
                State = (int)MoverEnemy.state,
                T = MoverEnemy.t,
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
            MoverEnemy.applyDamage(Atk, true);
        }
    }

    public class EnemySynchronizerHost : EnemySynchronizer
    {
        public void Update()
        {
            SendUpdateToAllPeers();
        }

        public void SendUpdateToAllPeers()
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.UpdateEnemyInfo,
                PeerId = WNMNTools.LocalID,
                UpdateEnemyInfo = GetEnemyInfo()
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            WNMNTools.peer.SendToAll(buffer, LiteNetLib.DeliveryMethod.Unreliable);
        }
    }

    public class EnemySynchronizerClient : EnemySynchronizer
    {
        public void UpdateEnemyInfo(UpdateEnemyInfo info)
        {
            MoverEnemy.setTo(info.PositionX, info.PositionY);
            MoverEnemy.Phy.killSpeedForce(true, true, true, true, true);

            var anim = MoverEnemy.getAnimator();
            anim.setPose(info.Pose);

            MoverEnemy.setAim((AIM)info.Aim);
            MoverEnemy.hp = info.Hp;
            MoverEnemy.mp = info.Mp;
            MoverEnemy.state = (NelEnemy.STATE)info.State;
            MoverEnemy.t = info.T;
        }
    }
}
