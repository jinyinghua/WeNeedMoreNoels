using HarmonyLib;
using m2d;
using nel;

namespace WeNeedMoreNoels
{
    //NelAttackInfo Atk, ref HITTYPE add_hittype, bool force, string fade_key = "", bool decline_ui_additional_effect = false, bool from_press_damage = false
    [HarmonyPatch(typeof(M2PrADmg), nameof(M2PrADmg.applyDamage), [typeof(NelAttackInfo), typeof(HITTYPE), typeof(bool), typeof(string), typeof(bool), typeof(bool)], [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal])]
    public class Patch_M2PrADmg_applyDamage
    {
        [HarmonyPrefix]
        static bool Prefix(object __instance)
        {
            return true;//__instance is PRNoel;
        }
    }
}
