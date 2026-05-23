using HarmonyLib;
using nel;
using nel.smnp;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(SummonerPlayer), nameof(SummonerPlayer.summonNewEnemy))]
    public class Patch_SummonerPlayer_summonNewEnemy
    {
        [HarmonyPostfix]
        static void Postfix(ref NelEnemy __result)
        {
            DB.CurEnemies.Add(__result);
            if (DB.IsMultiplayer)
            {
                if (WNMNTools.Type == NetWorkType.Host)
                {
                    __result.gameObject.AddComponent<EnemySynchronizerHost>();
                }
                else
                {
                    __result.gameObject.AddComponent<EnemySynchronizerClient>();
                }
            }
        }
    }
}
