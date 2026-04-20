using HarmonyLib;
using m2d;
using nel;

namespace WeNeedMoreNoels.Patch
{//NelAttackInfo Atk, ref HITTYPE add_hittype, bool force
    [HarmonyPatch(typeof(PR), nameof(PR.applyDamage), [typeof(NelAttackInfo), typeof(HITTYPE), typeof(bool)], [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal])]
    public class Patch_PR_applyDamage
    {
        [HarmonyPrefix]
        static void Prefix(object __instance, NelAttackInfo Atk, bool force)
        {
            if (__instance is ShadowNoel noel)
            {
                noel.OnNoelDamage?.Invoke(noel.ID, new()
                {
                    hp = Atk._hpdmg,
                    mp = Atk._mpdmg
                });
            }
        }
    }
}
