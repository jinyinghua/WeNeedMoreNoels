using HarmonyLib;
using nel;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(MgItemBomb), nameof(MgItemBomb.run))]
    public class Patch_MgItemBomb_run
    {
        [HarmonyPostfix]
        static void Postfix(MagicItem Mg)
        {
            if (Mg.Caster is not PRNoel)
            {
                return;
            }
            if (Mg.Dro == null)
            {
                NotifyNoelMagic mg1 = new()
                {
                    Type = NotifyMagicTpe.RemoveBomb
                };
                WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg1);
                return;
            }
            NotifyNoelMagic mg = new()
            {
                Type = NotifyMagicTpe.UpdateBomb,
                T = Mg.t,
                Phase = Mg.phase,
                BombX = Mg.Dro.x,
                BombY = Mg.Dro.y
            };
            WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg);
        }
    }
}
