using m2d;
using nel;
using System;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels.SN
{
    public class ShadowNoel : PRMain
    {
        public ClientConfig InitConfig;
        public int ID;
        public int PartyID;

        public bool ChantMagic;
        public float MagicAim;

        public AIM CurAim;
        public STATE CurState;

        public Action<int, NotifyNoelDamage> OnNoelDamage;

        public ShadowNoelNickname NicknameIns;

        public void CreateNicknameWithNoel(string nickname)
        {
            getPosition(out float x, out float y);
            ShadowNoelNickname follower = Mp.createMover<ShadowNoelNickname>($"Nickname_{nickname}", x, y);
            follower.SetFollowTarget(this, new Vector2(0f, -2f));
            follower.SetText(nickname);
            follower.SetTextSize(20f);
            follower.SetTextColor(uint.MaxValue);
            follower.SetBorderColor(4278190080U);
            follower.SetTextOffset(0f, -50f);
            follower.SetAlpha(1);
            DB.MainPR.Mp.assignMover(follower);
            follower.appear(DB.MainPR.Mp);
            NicknameIns = follower;
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void newGame()
        {
            this.hp = (this.maxhp = 150);
            this.mp = (this.maxmp = 200);
            this.EpCon ??= new EpManager(this);
            if (base.VO == null)
            {
                base.VO = new PrVoiceController(this, MTR.VcNoelSource, this.snd_key + ".voice");
                this.BetoMng = BetobetoManager.GetManager("noel");
            }
            base.newGame();
            this.Ser.clear();
            this.EpCon.newGame();
            this.EggCon.newGame(false);
            this.GaugeBrk.reset();
            base.key = "shadow_noel";
            this.AbsorbCon = new AbsorbManagerContainer(5, this);
        }

        public override M2Mover setAim(AIM n, bool sprite_force_aim_set = false)
        {
            return this;
        }

        public override void changeState(STATE state, STATE prestate) { }

        public override void createAnimator(ref PrAnimator Anm)
        {
            if (InitConfig is not null)
            {
                M2PxlAnimatorRT m2PxlAnimatorRT;
                if (Anm == null)
                {
                    SfPose = new AnimationShuffler(this);
                    PrPoseContainer container;
                    switch (InitConfig.NoelType)
                    {
                        case NoelType.Normal:
                            m2PxlAnimatorRT = this.Mp.M2D.createBasicPxlAnimatorForRenderTicket(this, "noel_magic", "stand", false, M2Mover.DRAW_ORDER.PR1);
                            container = MTR.PConNoelAnim;
                            container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTR.Anoel_pxls, 56f, CaneManager.DefaultCane);
                            break;
                        case NoelType.Inverse:
                            m2PxlAnimatorRT = this.Mp.M2D.createBasicPxlAnimatorForRenderTicket(this, "noel_inverse_magic", "stand", false, M2Mover.DRAW_ORDER.PR1);
                            container = MTRExtension.PConNoelIAnim;
                            container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTRExtension.Anoel_inverse_pxls, 56f, CaneManager.DefaultCane);
                            break;
                        case NoelType.ColorNoel:
                            m2PxlAnimatorRT = this.Mp.M2D.createBasicPxlAnimatorForRenderTicket(this, MTRExtension.GetColorNoelName(InitConfig.NoelColor), "stand", false, M2Mover.DRAW_ORDER.PR1);
                            container = MTRExtension.GetPrPoseContainer(InitConfig.NoelColor);
                            container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTRExtension.GetColorNoelPxls(InitConfig.NoelColor), 56f, CaneManager.DefaultCane);
                            break;
                        default:
                            return;
                    }
                    AnmN = new ShadowNoelAnimator(this, m2PxlAnimatorRT, container, false);
                    Anm = AnmN;
                    AnmN.initS(m2PxlAnimatorRT);
                    return;
                }
            }
        }

        public override void appear(Map2d Mp)
        {
            DB.ShadowAppear = true;
            base.appear(Mp);
            DB.ShadowAppear = false;

            this.UP?.destruct();
            this.UP = null;
        }

        public override void refineMoveKey(bool ignore_keypushdown = false) { }

        public override bool runUi() {
            var tg = this.Mp.TalkTarget_;
            bool rt = true;
            rt = base.runUi();
            if (tg != this.Mp.TalkTarget_) {
                this.Mp.setTalkTarget(tg);
            }
            return rt;
        }

        public override void runPre()
        {
            Skill.magic_t = (float)Skill.MAGIC_CHANT_DELAY;
            base.runPre();
            base.setAim(CurAim);
            if (state != CurState)
            {
                base.changeState(CurState, state);
            }
        }

        public override void runPost()
        {
            base.runPost();
            Phy.killSpeedForce(true, true, true, true, true);
        }

        public override HITTYPE getHitType(M2Ray Ray)
        {
            return HITTYPE.EN;
        }

        public override void deactivateFromMap()
        {
            base.deactivateFromMap();
            Mp.removeMover(NicknameIns);
            NicknameIns.destruct();
            DB.noelIns[ID].NicknameIns = null;
        }

        public void ReawakeMagic(MGKIND kind, float t)
        {
            Skill.reawakeMagic(kind);
            Skill.CurMg.castedTimeResetTo(t);
        }

        public void SleepMagic()
        {
            Skill.CurMg.Sleep(false);
        }

        public void KillMagic()
        {
            if (Skill.CurMg is null)
            {
                return;
            }
            Skill.CurMg.close(true);
            Skill.CurMg.kill(-1f);
            Skill.OcSlots.clearMagic(Skill.CurMg, false);
            Skill.CurMg = null;
            Skill.MagicSel.deactivate();
        }

        private PrAnimator AnmN;
    }
}
