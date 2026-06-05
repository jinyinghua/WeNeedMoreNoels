using HarmonyLib;
using nel;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(MgWaterShard), nameof(MgWaterShard.fnManipulateWaterShard))]
    public class Patch_MgWaterShard_fnManipulateWaterShard
    {
        [HarmonyPostfix]
        static void Postfix(MgWaterShard __instance, MagicItem Mg, M2MagicCaster _Mv)
        {
            if (_Mv is not PRNoel)
            {
                return;
            }
            MgWaterShard.IdAndPhase(Mg, out int id, out int phase);
            if (phase != 500)
            {
                return;
            }
            NotifyNoelMagic mg = new()
            {
                Type = NotifyMagicTpe.WaterShoot,
                agR = Mg.aim_agR,
                id = id
            };
            WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg);
        }
    }
}
