using evt;
using m2d;
using nel;
using System.Linq;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels.SN
{
    public static class ShadowNoelExtensions
    {
        public static ShadowNoel GenerateShadowNoel(ClientConfig config, int id = -1)
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
                    noel.PartyID = DB.noelIns[id].NoelInfo.PartyID;
                    noel.OnNoelDamage = WNMNTools.SendDamageToAllPeers;
                    if (DB.InitConfig.InvisibleNickname)
                    {
                        noel.CreateNicknameWithNoel(TX.Get("multiplayer_noel_nickname") + id.ToString());
                    }
                    else
                    {
                        noel.CreateNicknameWithNoel(DB.noelIns[id].NickNameStr);
                    }
                    DB.noelIns[id].NicknameIns = noel.NicknameIns;
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
            noel.PartyID = DB.partyInfos[id].ID;
            noel.OnNoelDamage = WNMNTools.SendDamageToAllPeers;
            DB.noelIns.Add(id, new()
            {
                Noel = noel,
                Nickname = config.Nickname,
                MpKey = map.key,
                NoelInitConfig = config,
                NoelInfo = GetSendInfo(),
                Enabled = true,
                NicknameIns = noel.NicknameIns,
                ID = id
            });
            if (DB.InitConfig.InvisibleNickname)
            {
                noel.CreateNicknameWithNoel(TX.Get("multiplayer_noel_nickname") + id.ToString());
            }
            else
            {
                noel.CreateNicknameWithNoel(DB.noelIns[id].NickNameStr);
            }
            DB.noelIns[id].NicknameIns = noel.NicknameIns;
            return noel;
        }

        public static void GenerateMainPRNickname(string nickname)
        {
            Map2d Mp = DB.MainPR.Mp;
            DB.MainPR.getPosition(out float x, out float y);
            ShadowNoelNickname follower = Mp.createMover<ShadowNoelNickname>($"Nickname_{nickname}", x, y);
            follower.SetFollowTarget(DB.MainPR, new Vector2(0f, -2f));
            follower.SetText(nickname);
            follower.SetTextSize(20f);
            follower.SetTextColor(uint.MaxValue);
            follower.SetBorderColor(4278190080U);
            follower.SetTextOffset(0f, -50f);
            follower.SetAlpha(1);
            DB.MainPR.Mp.assignMover(follower);
            follower.appear(DB.MainPR.Mp);
            DB.MainPRNickname = follower;
            GenerateMainPRMsg();
        }

        public static void GenerateMainPRMsg()
        {
            Map2d Mp = DB.MainPR.Mp;
            DB.MainPR.getPosition(out float x, out float y);
            ShadowNoelNickname follower = Mp.createMover<ShadowNoelNickname>($"Msg_{DB.MainPR}", x, y);
            follower.SetFollowTarget(DB.MainPR, new Vector2(0f, -2f));
            follower.SetTextSize(20f);
            follower.SetTextColor(uint.MaxValue);
            follower.SetBorderColor(4278190080U);
            follower.SetTextOffset(0f, 0f);
            follower.SetAlpha(1);
            follower.SetBgColor(new(0, 0, 0, 0));
            DB.MainPR.Mp.assignMover(follower);
            follower.appear(DB.MainPR.Mp);
            DB.MainPRMsg = follower;
        }

        public static void UpdateShadowNoelInfo(int id)
        {
            ShadowNoelInstance ins = DB.noelIns[id];
            UpdateNoelInfo info = ins.NoelInfo;
            ins.MpKey = info.MpKey;
            if (ins.MpKey != DB.MainPR.Mp.key)
            {
                DisableShadowNoel(id);
                return;
            }
            else
            {
                EnableShadowNoel(id);
            }
            ShadowNoel noel = ins.Noel;
            MoveShadowNoel(noel, new(info.PositionX, info.PositionY));
            SetPoseShadowNoel(noel, info.Pose, (AIM)info.Aim);
            SetHPMP(noel, info.Hp, info.Mp);
            if (noel.getSkillManager().getCurrentCaneEquip().GetItem().id != info.CaneItemId)
            {
                SetCane(noel, (ushort)info.CaneItemId, (byte)info.CaneGrade);
            }
            if (noel.CurState != (PR.STATE)info.State)
            {
                noel.CurState = (PR.STATE)info.State;
            }
            noel.PartyID = info.PartyID;
            if (noel.PartyID != DB.LocalNoelParty)
            {
                EnableShadowNoelHit(noel);
            }
            else
            {
                DisableShadowNoelHit(noel);
            }
            noel.ChantMagic = info.ChantMagic;
            noel.MagicAgR = info.MagicAgR;
            noel.Skill.mp_hold = info.MagicHold;
            noel.MagicT = info.MagicT;
            noel.MagicHoldAim = info.MagicHoldAim;
        }

        public static void DisableShadowNoel(int id)
        {
            if (!DB.noelIns.ContainsKey(id))
            {
                Plugin.Logger.LogWarning("try to disable not existing noel");
                return;
            }
            if (!DB.noelIns[id].Enabled)
            {
                return;
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
                return;
            }
            if (DB.noelIns[id].Enabled)
            {
                return;
            }
            ShadowNoel noel = GenerateShadowNoel(DB.noelIns[id].NoelInitConfig, id);
            DB.noelIns[id].Enabled = true;
            DB.noelIns[id].Noel = noel;
        }

        public static void MoveShadowNoel(ShadowNoel noel, System.Numerics.Vector2 pos)
        {
            if (noel.Phy is null)
            {
                return;
            }
            noel.setTo(pos.X, pos.Y);
            noel.Phy.killSpeedForce(true, true, true, true, true);
        }

        public static void SetPoseShadowNoel(ShadowNoel noel, string pose, AIM aim)
        {
            noel.setAim(aim);
            ShadowNoelAnimator Anm = (ShadowNoelAnimator)noel.Anm;
            if (Anm.pose_title == pose)
            {
                return;
            }
            Anm.setPose(pose);
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

        public static void DamageNoel(int id, NotifyNoelDamage dmg)
        {
            var Atk = new NelAttackInfo
            {
                attr = MGATTR.NORMAL,
                ndmg = NDMG.DEFAULT,
                hpdmg0 = dmg.Hp,
                mpdmg0 = dmg.Mp,
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

        public static void SetNoelMagic(int id, NotifyNoelMagic mg)
        {
            switch (mg.Type)
            {
                case NotifyMagicTpe.Reawake:
                    DB.noelIns[id].Noel.ReawakeMagic((MGKIND)mg.Kind, mg.T);
                    if (DB.MNBridge.ContainsKey(DB.noelIns[id].Noel.Skill.CurMg))
                    {
                        break;
                    }
                    DB.MNBridge.Add(DB.noelIns[id].Noel.Skill.CurMg, DB.noelIns[id].Noel);
                    break;
                case NotifyMagicTpe.Sleep:
                    DB.noelIns[id].Noel.SleepMagic();
                    break;
                case NotifyMagicTpe.Kill:
                    if (DB.MNBridge.Count == 0)
                    {
                        break;
                    }
                    DB.MNBridge.Remove(DB.MNBridge.First(x => x.Value == DB.noelIns[id].Noel).Key);
                    DB.noelIns[id].Noel.KillMagic();
                    break;
            }
        }

        public static void StartCurMapBattle()
        {
            if (M2LpSummon.NearLpSmn is not null)
            {
                DB.CurSummoner = M2LpSummon.NearLpSmn;
                DB.CurEnemies.Clear();
                M2LpSummon.NearLpSmn.openSummoner(DB.MainPR);
            }
        }

        public static void EndCurMapBattle()
        {
            if (DB.CurSummoner is not null)
            {
                foreach (NelEnemy enemy in DB.CurEnemies)
                {
                    DB.MainPR.Mp.removeMover(enemy);
                    enemy.destruct();
                }
                DB.CurEnemies.Clear();
                DB.CurSummoner.closeSummoner(true, out _);
                DB.CurSummoner = null;
            }
        }

        public static void DisableShadowNoelHit(ShadowNoel noel)
        {
            noel.gameObject.tag = "MoverPr";
            noel.gameObject.layer = 0; //Default
        }

        public static void EnableShadowNoelHit(ShadowNoel noel)
        {
            noel.gameObject.tag = "MoverEn";
            noel.gameObject.layer = 23; //23 is EmenyLayer
        }

        public static UpdateNoelInfo GetSendInfo()
        {
            DB.MainPR.getPosition(out float x, out float y);
            PrNoelAnimator Anm = DB.MainPR.AnmN;
            M2PrSkill skill = DB.MainPR.getSkillManager();
            MagicItem item = skill.CurMg;
            PrCaneEquip cane = skill.getCurrentCaneEquip();
            return new()
            {
                PositionX = x,
                PositionY = y,
                Pose = Anm.pose_title,
                Aim = Anm.pose_aim,
                IsCrouch = DB.MainPR.is_crouch,
                Hp = DB.MainPR.hp,
                Mp = DB.MainPR.mp,
                State = (int)DB.MainPR.state,
                CaneItemId = cane.GetItem().id,
                CaneGrade = cane.grade,
                PartyID = DB.LocalNoelParty,
                MpKey = DB.MainPR.Mp.key,
                ChantMagic = DB.MainPR.magic_chanting,
                MagicAgR = item is null ? 0 : item.aim_agR,
                MagicHold = skill.mp_hold,
                MagicT = skill.magic_t,
                MagicHoldAim = skill.Cursor.pre_hold_aim
            };
        }
    }
}
