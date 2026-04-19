using m2d;
using nel;
using System;

namespace WeNeedMoreNoels
{
    public class ShadowNoel : PRMain
    {
        public WNMNTools.NetworkConfig InitConfig;

        public int ID;

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
                            m2PxlAnimatorRT = this.Mp.M2D.createBasicPxlAnimatorForRenderTicket(this, "noel", "stand", false, M2Mover.DRAW_ORDER.PR1);
                            container = MTR.PConNoelAnim;
                            container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTR.Anoel_pxls, 56f, CaneManager.DefaultCane);
                            break;
                        case NoelType.Inverse:
                            m2PxlAnimatorRT = this.Mp.M2D.createBasicPxlAnimatorForRenderTicket(this, "noel_inverse", "stand", false, M2Mover.DRAW_ORDER.PR1);
                            container = MTRExtension.PConNoelIAnim;
                            container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTRExtension.Anoel_inverse_pxls, 56f, CaneManager.DefaultCane);
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

        public override void runPost()
        {
            try
            {
                base.runPost();
                Phy.killSpeedForce(true, true, true, true, true);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        private PrAnimator AnmN;
    }
}
