using HarmonyLib;
using m2d;
using nel;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PRNoel), nameof(PRNoel.createAnimator))]
    public class Patch_PRNoel_createAnimator
    {
        [HarmonyPrefix]
        static bool Prefix(object __instance, ref PrAnimator Anm)
        {
            if (DB.InitConfig is null)
            {
                return true;
            }
            PRNoel noel = (PRNoel)__instance;
            M2PxlAnimatorRT m2PxlAnimatorRT;
            noel.SfPose = new AnimationShufflerNoel(noel);
            PrPoseContainer container;
            switch (DB.InitConfig.NoelType)
            {
                case NoelType.Normal:
                    m2PxlAnimatorRT = noel.Mp.M2D.createBasicPxlAnimatorForRenderTicket(noel, "noel", "stand", false, M2Mover.DRAW_ORDER.PR1);
                    container = MTR.PConNoelAnim;
                    container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTR.Anoel_pxls, 56f, CaneManager.DefaultCane);
                    break;
                case NoelType.Inverse:
                    m2PxlAnimatorRT = noel.Mp.M2D.createBasicPxlAnimatorForRenderTicket(noel, "noel_inverse", "stand", false, M2Mover.DRAW_ORDER.PR1);
                    container = MTRExtension.PConNoelIAnim;
                    container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTRExtension.Anoel_inverse_pxls, 56f, CaneManager.DefaultCane);
                    break;
                case NoelType.ColorNoel:
                    m2PxlAnimatorRT = noel.Mp.M2D.createBasicPxlAnimatorForRenderTicket(noel, MTRExtension.GetColorNoelName(DB.InitConfig.NoelColor), "stand", false, M2Mover.DRAW_ORDER.PR1);
                    container = MTRExtension.GetPrPoseContainer(DB.InitConfig.NoelColor);
                    container.iniPxlResourcesASync<PRNoel.OUTFIT>(MTRExtension.GetColorNoelPxlsFull(DB.InitConfig.NoelColor), 56f, CaneManager.DefaultCane);
                    break;
                default:
                    return false;
            }
            noel.AnmN = new ShadowNoelAnimator(noel, m2PxlAnimatorRT, container, false);
            Anm = noel.AnmN;
            noel.AnmN.initS(m2PxlAnimatorRT);
            return false;
        }
    }
}
