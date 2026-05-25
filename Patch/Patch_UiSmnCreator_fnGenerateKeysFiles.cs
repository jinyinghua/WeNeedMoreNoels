using HarmonyLib;
using nel.mgm.smncr;
using System.Collections.Generic;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.fnGenerateKeysFiles))]
    public class Patch_UiSmnCreator_fnGenerateKeysFiles
    {
        [HarmonyPostfix]
        static void Postfix(UiSmnCreator __instance, List<string> Adest)
        {
            Adest.Add("&&multiplayer_simbattle_btn");
        }
    }
}
