using m2d;
using nel;
using PixelLiner;
using XX;

namespace WeNeedMoreNoels
{
    public class ShadowNoel : PRMain
    {        
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

        public override void createAnimator(ref PrAnimator anm)
        {
            M2PxlAnimatorRT m2PxlAnimatorRT = this.Mp.M2D.createBasicPxlAnimatorForRenderTicket(this, "noel", "stand", false);
            if (Anm == null)
            {
                this.SfPose = new AnimationShuffler(this);
                PrPoseContainer container = new PrPoseContainer("shadow_noel", delegate (PxlFrame F, float rCLENB)
                {
                    float num3;
                    float num4;
                    return M2PxlAnimator.getRodPosS(rCLENB, F, out num3, out num4, "rod", "ROD", 0.5f, 0f, ALIGN.LEFT, ALIGNY.MIDDLE, 2, "rodeff");
                });
                container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTR.Anoel_pxls, 56f, CaneManager.DefaultCane);
                this.AnmN = new ShadowNoelAnimator(this, m2PxlAnimatorRT, container, false);
                this.AnmN.initS(m2PxlAnimatorRT);
                Anm = this.AnmN;
                return;
            }
            Anm.initS(m2PxlAnimatorRT);
        }

        public override void appear(Map2d Mp)
        {
            DB.ShadowAppear = true;
            base.appear(Mp);
            DB.ShadowAppear = false;

            this.UP?.destruct();
            this.UP = null;
        }

        public override bool isDamagingOrKo()
        {
            return false;
        }

        public override void refineMoveKey(bool ignore_keypushdown = false) { }

        public override void deactivateFromMap()
        {
            if (delete)
                return;
            delete = true;
            this.Mp.destructPxlAnimByMover(this);
            this.Mp.removeMover(this);
            this.destruct();
            base.deactivateFromMap();
        }

        private PrAnimator AnmN;

        private bool delete;
    }
}
