using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(nel.title.SceneTitleTemp), "initButtons")]
    public class Patch_SceneTitleTemp_initButtons
    {
        [HarmonyPrefix]
        static void Prefix(object __instance)
        {
            var field = AccessTools.Field(__instance.GetType(), "Atop_btn_keys");
            var oldArray = field.GetValue(__instance) as string[];
            string newStr = "&&btn_multiplayer";
            string[] newArray = new string[oldArray.Length + 1];
            oldArray.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = newArray[newArray.Length - 2];
            newArray[newArray.Length - 2] = newArray[newArray.Length - 3];
            newArray[newArray.Length - 3] = newStr;
            field.SetValue(__instance, newArray);
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codeMatcher = new CodeMatcher(instructions);
            codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Stfld),
                                          new CodeMatch(OpCodes.Dup),
                                          new CodeMatch(OpCodes.Ldc_I4_4))
                       .Advance(2)
                       .SetOpcodeAndAdvance(OpCodes.Ldc_I4_5);
            codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Sub),
                                          new CodeMatch(OpCodes.Ldc_R4),
                                          new CodeMatch(OpCodes.Sub),
                                          new CodeMatch(OpCodes.Ldc_R4))
                       .Advance(3)
                       .SetOperandAndAdvance(5f);
            codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldfld),
                                          new CodeMatch(OpCodes.Ldc_I4_4))     // 匹配 ldc.i4.4
                       .Advance(1)
                       .SetOpcodeAndAdvance(OpCodes.Ldc_I4_5);                 // 将 ldc.i4.4 改为 ldc.i4.5
            return codeMatcher.Instructions();
        }
    }
}
