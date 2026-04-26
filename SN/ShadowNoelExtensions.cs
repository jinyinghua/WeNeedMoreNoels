using m2d;
using nel;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using XX;
using static evt.ManpuDrawer;
using static nel.NelNPentapodHead;
using static UnityEngine.GraphicsBuffer;

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
                    noel.CreateNicknameWithNoel(config.Nickname);
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
            PartyManager.Party party = PartyManager.InitNewParty(id);
            noel.ID = id;
            noel.PartyID = party.ID;
            noel.OnNoelDamage = WNMNTools.SendDamageToAllPeers;
            noel.CreateNicknameWithNoel(config.Nickname);
            DB.noelIns.Add(id, new()
            {
                Noel = noel,
                Nickname = config.Nickname,
                MpKey = map.key,
                NoelInitConfig = config,
                NoelInfo = GetSendInfo(),
                Enabled = true,
                NicknameIns = noel.NicknameIns
            });
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
            if (noel.state != (PR.STATE)info.State)
            {
                noel.changeState((PR.STATE)info.State);
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
            PrCaneEquip cane = DB.MainPR.getSkillManager().getCurrentCaneEquip();
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
                MpKey = DB.MainPR.Mp.key
            };
        }
    }
}
