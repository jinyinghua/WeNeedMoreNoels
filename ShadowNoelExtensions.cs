using m2d;
using nel;
using System.Collections.Generic;
using UnityEngine;
using WeNeedMoreNoels.HostMessages;
using XX;

namespace WeNeedMoreNoels
{
    public static class ShadowNoelExtensions
    {
        public static ShadowNoel GenerateShadowNoel(WNMNTools.NetworkConfig config, int id = -1)
        {
            Map2d map = M2DBase.Instance.curMap;
            map.Pr.getPosition(out float x, out float y);
            ShadowNoel noel;
            if (DB.noelIns.ContainsKey(id))
            {
                if (!DB.noelIns[id].Enabled)
                {
                    noel = map.createMover<ShadowNoel>("ShadowNoel", x, y);
                    noel.InitConfig = config;
                    noel.newGame();
                    noel.gameObject.AddComponent<Rigidbody2D>();
                    noel.gameObject.name = "ShadowNoel";
                    map.assignMover(noel);
                    noel.ID = id;
                    DB.noelIns[id].Noel = noel;
                    return noel;
                }
                return null;
            }
            noel = map.createMover<ShadowNoel>("ShadowNoel", x, y);
            noel.InitConfig = config;
            noel.newGame();
            noel.gameObject.AddComponent<Rigidbody2D>();
            noel.gameObject.name = "ShadowNoel";
            map.assignMover(noel);
            noel.ID = id;
            DB.noelIns.Add(id, new()
            {
                Noel = noel,
                Nickname = config.nickName,
                MpKey = map.key,
                NoelInitConfig = config,
                NoelInfo = new()
            });
            return noel;
        }

        public static void DisableShadowNoel(int id)
        {
            if (!DB.noelIns.ContainsKey(id))
            {
                Plugin.Logger.LogWarning("try to disable not existing noel");
            }
            ShadowNoel noel = DB.noelIns[id].Noel;
            noel.Mp.destructPxlAnimByMover(noel);
            noel.Mp.removeMover(noel);
            noel.destruct();
            Object.DestroyImmediate(noel.gameObject);
            DB.noelIns[id].Enabled = false;
            DB.noelIns[id].Noel = null;
        }

        public static void EnableShadowNoel(int id)
        {
            if (!DB.noelIns.ContainsKey(id))
            {
                Plugin.Logger.LogWarning("try to disable not existing noel");
            }
            GenerateShadowNoel(DB.noelIns[id].NoelInitConfig, id);
        }

        public static void UpdateShadowInfo()
        {

        }

        public static void MoveShadowNoel(ShadowNoel noel, System.Numerics.Vector2 pos)
        {
            if (noel.Phy is null)
            {
                return;
            }
            noel.getPosition(out float x, out float y);
            float dx = pos.X - x;
            float dy = pos.Y - y;
            noel.setTo(pos.X, pos.Y);
            noel.Phy.killSpeedForce(true, true, true, true, true);
        }

        public static void SetPoseShadowNoel(ShadowNoel noel, string pose, AIM aim)
        {
            ShadowNoelAnimator Anm = (ShadowNoelAnimator)noel.Anm;
            if (Anm.pose_title == pose)
            {
                return;
            }
            Anm.setPose(pose);
            Anm.setAim(aim);
            noel.Skill.setAim(aim);
        }

        public static void SetHPMP(ShadowNoel noel, int hp, int mp)
        {
            noel.hp = hp;
            noel.mp = mp;
        }

        public static void SetCane(ShadowNoel noel, ushort key, byte grade)
        {
            NelItem item = NelItem.GetByUId(key, true);
            if (item == null)
                return;
            CaneManager.CaneItem cane = CaneManager.Get(item, true);
            if (cane == null)
                return;
            if (noel.getSkillManager().getCurrentCaneEquip().cane_key == cane.key)
            {
                return;
            }
            noel.getSkillManager().switchCane(cane, grade, false);
        }

        public static void DamageNoel(int id, ShadowNoelDamage dmg)
        {
            var Atk = new NelAttackInfo
            {
                attr = MGATTR.NORMAL,
                ndmg = NDMG.DEFAULT,
                hpdmg0 = dmg.hp,
                mpdmg0 = dmg.mp,
                fix_damage = true,
                parryable = false,
                shield_break_ratio = 1f,
                ignore_nodamage_time = true,
                nodamage_time = 0,
            };
            if (WNMNTools.Type == NetWorkType.Host && id == 0)
            {
                DB.MainPR.DMG.applyDamage(Atk, true);
            }
            else
            {
                if (id == WNMNTools.LocalID)
                {
                    DB.MainPR.DMG.applyDamage(Atk, true);
                }
                else
                {
                    DB.noelIns[id].Noel.DMG.applyDamage(Atk, true);
                }
            }
        }

        public static void DisableAllShadowNoels()
        {
            foreach (var pair in DB.noelIns)
            {
                DisableShadowNoel(pair.Key);
            }
        }

        public static void DetectShadowNoelInCurrentMap()
        {
            foreach (var pair in DB.noelIns)
            {
                if (DB.MainPR.Mp.key == pair.Value.MpKey)
                {
                    EnableShadowNoel(pair.Key);
                }
            }
        }

        public static void DisableShadowNoelHit(ShadowNoel noel)
        {

        }

        public static void EnableShadowNoelHit(ShadowNoel noel)
        {
            noel.gameObject.tag = "MoverEn";
            noel.gameObject.layer = 23; //23 is EmenyLayer
        }
    }
}
