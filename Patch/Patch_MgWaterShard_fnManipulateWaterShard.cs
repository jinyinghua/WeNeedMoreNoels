using HarmonyLib;
using nel;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(MgWaterShard), nameof(MgWaterShard.fnManipulateWaterShard))]
    public class Patch_MgWaterShard_fnManipulateWaterShard
    {
        [HarmonyPostfix]
        static void Postfix(MagicItem Mg, M2MagicCaster _Mv)
        {
            if (_Mv is not PRNoel)
            {
                return;
            }
            NotifyNoelMagic mg = new()
            {
                Type = NotifyMagicTpe.UpdateWater,
                da = Mg.da,
                phase = Mg.phase
            };
            WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg);
        }
    }
}
