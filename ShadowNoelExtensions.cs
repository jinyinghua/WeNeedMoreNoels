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
        public static ShadowNoel GenerateShadowNoel(int id = -1)
        {
            if (DB.noelEnables.ContainsKey(id) && DB.noelEnables[id])
            {
                return null;
            }
            Map2d map = M2DBase.Instance.curMap;
            map.Pr.getPosition(out float x, out float y);
            ShadowNoel noel = map.createMover<ShadowNoel>("ShadowNoel", x, y);
            noel.InitConfig = DB.noelConfigs[id];
            noel.newGame();
            noel.gameObject.AddComponent<Rigidbody2D>();
            noel.gameObject.name = "ShadowNoel";
            map.assignMover(noel);
            DB.noelDics.Add(id, noel);
            if (!DB.noelEnables.ContainsKey(id))
            {
                DB.noelEnables.Add(id, true);
            }
            else
            {
                DB.noelEnables[id] = true;
            }
            if (!DB.noelMpKeys.ContainsKey(id))
            {
                DB.noelMpKeys.Add(id, noel.Mp.key);
            }
            noel.ID = id;
            return noel;
        }

        public static void DisableShadowNoel(int id)
        {
            if (!DB.noelEnables.ContainsKey(id))
            {
                Plugin.Logger.LogWarning("try to disable not existing noel");
            }
            if (!DB.noelDics.ContainsKey(id))
            {
                return;
            }
            ShadowNoel noel = DB.noelDics[id];
            noel.Mp.destructPxlAnimByMover(noel);
            noel.Mp.removeMover(noel);
            noel.destruct();
            Object.DestroyImmediate(noel.gameObject);
            DB.noelDics.Remove(id);
            DB.noelEnables[id] = false;
        }

        public static void EnableShadowNoel(int id)
        {
            if (!DB.noelEnables.ContainsKey(id))
            {
                Plugin.Logger.LogWarning("try to disable not existing noel");
            }
            GenerateShadowNoel(id);
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
            noel.getSkillManager().switchCane(cane, (int)grade, false);
        }

        public static void UpdateShadowNoelMpKey(int id, string key)
        {
            DB.noelMpKeys[id] = key;
        }

        public static void UpdateShadowNoelState(int id, PR.STATE STATE)
        {
            DB.noelDics[id].changeState(STATE);
        }

        public static void CheckDBDics(int id, WNMNTools.NetworkConfig config)
        {
            if (!DB.noelNicknames.ContainsKey(id))
            {
                DB.noelNicknames.Add(id, config.nickName);
            }
            if (!DB.noelInfos.ContainsKey(id))
            {
                DB.noelInfos.Add(id, new());
            }
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
                nodamage_time = 0
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
                    DB.noelDics[id].DMG.applyDamage(Atk, true);
                }
            }
        }

        public static void DisableAllShadowNoels()
        {
            List<int> disabledNoels = [];
            foreach (var pair in DB.noelMpKeys)
            {
                disabledNoels.Add(pair.Key);
            }
            foreach (int key in disabledNoels)
            {
                DisableShadowNoel(key);
            }
        }

        public static void DetectShadowNoelInCurrentMap()
        {
            foreach (var pair in DB.noelMpKeys)
            {
                if (DB.MainPR.Mp.key == pair.Value)
                {
                    EnableShadowNoel(pair.Key);
                }
            }
        }

        //public static void WalkDummy(ShadowNoel noel, float t, AIM aim, float speed)
        //{
        //    noel.StartCoroutine(WalkCoroutine(noel, aim, speed, t));
        //}

        //static IEnumerator WalkCoroutine(ShadowNoel noel, AIM aim, float speed, float targetTime)
        //{
        //    float timer = 0;
        //    ShadowNoelAnimator anm = (ShadowNoelAnimator)noel.Anm;
        //    anm.setPose(WALK_STATE);
        //    while (timer < targetTime)
        //    {
        //        noel.walkByAim((int)aim, speed);
        //        timer += Time.deltaTime;
        //        yield return null;
        //    }
        //    anm.setPose(STAND_STATE);
        //    yield return null;
        //}

        const string WALK_STATE = "walk";

        const string STAND_STATE = "stand";
    }
}
