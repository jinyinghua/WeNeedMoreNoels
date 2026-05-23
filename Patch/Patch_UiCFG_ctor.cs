using HarmonyLib;
using nel;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using XX;

namespace WeNeedMoreNoels.Patch
{
    //[HarmonyDebug]
    [HarmonyPatch(typeof(UiCFG), MethodType.Constructor,
        [typeof(UiBoxDesigner), typeof(UiBoxDesigner), typeof(Designer), typeof(bool), typeof(bool), typeof(UiCFG.FnCfgTabCreateAfter), typeof(bool)])]
    public class Patch_UiCFG_ctor
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var get_IsMultiplayer = AccessTools.Method(typeof(DB), "get_" + nameof(DB.IsMultiplayer));
            var isSpActivated = AccessTools.Method(typeof(CFGSP), nameof(CFGSP.isSpActivated));
            var concat = AccessTools.Method(typeof(String), nameof(String.Concat), [typeof(string), typeof(string), typeof(string)]);

            var matcher = new CodeMatcher(instructions, generator)
                .MatchEndForward(new CodeMatch(OpCodes.Call, isSpActivated))    // if (CFG.isSpActivated())
                .Advance(1)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, get_IsMultiplayer),       // => if (CFG.isSpActivated() || DB.IsMultiplayer)
                    new CodeInstruction(OpCodes.Or)
                )

                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldc_I4_2),
                    new CodeMatch(OpCodes.Newarr, typeof(string))               // new string[2]
                )
                .RemoveInstruction()
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, isSpActivated),           // => new string[1 + (CFG.isSpActivated() ? 1 : 0) + (DB.IsMultiplayer ? 1 : 0)]
                    new CodeInstruction(OpCodes.Call, get_IsMultiplayer),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add)
                )

                .MatchEndForward(
                    new CodeMatch(OpCodes.Call, concat),
                    new CodeMatch(OpCodes.Stelem_Ref)
                )
                .Advance(1)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, isSpActivated),           // + if (CFG.isSpActivated())
                    new CodeInstruction(OpCodes.Break)          // placeholder
                )
                .MatchEndForward(
                    new CodeMatch(OpCodes.Call, concat),
                    new CodeMatch(OpCodes.Stelem_Ref)
                )
                .Advance(1)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, get_IsMultiplayer),       // + if (DB.IsMultiplayer)
                    new CodeInstruction(OpCodes.Break),         // placeholder
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Call, isSpActivated),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add),                           // keys[(CFG.isSpActivated() ? 2 : 1)] = ...
                    new CodeInstruction(OpCodes.Ldstr, """<img mesh="mini_checkbox_checked" width="20" height="24" color="0xff554c4a" />"""),
                    new CodeInstruction(OpCodes.Stelem_Ref)
                )
                .CreateLabel(out var label)
                .MatchStartBackwards(new CodeMatch(OpCodes.Break))
                .Set(OpCodes.Brfalse, label)
                .Advance(-1)    // call get_IsMultiplayer
                .CreateLabel(out label)
                .MatchStartBackwards(new CodeMatch(OpCodes.Break))
                .Set(OpCodes.Brfalse, label);

                //.MatchStartForward(
                //    new CodeMatch(OpCodes.Ldarg_0),
                //    new CodeMatch(OpCodes.Ldstr, "MAIN"),
                //    new CodeMatch(OpCodes.Ldc_I4_0),
                //    new CodeMatch(OpCodes.Ldc_I4_1),
                //    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UiCFG), nameof(UiCFG.fineTabVisibility)))
                //)
                //.Advance(1)
                //.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                //.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                //.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                //.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop));

            return matcher.InstructionEnumeration();
        }

        [HarmonyPostfix]
        static void Postfix(UiCFG __instance)
        {
            if (DB.IsMultiplayer)
            {
                var w = __instance.OTab["MAIN"].w;    // idk but __instance.BxOut.use_w/h changed somewhere, get original value here
                var h = __instance.OTab["MAIN"].h;
                var designer = __instance.BxOut.addTab("_DsmInner_Mp", w, h, w, h, true);
                __instance.setBoxMainStencil();

                CFGMultiplayer.CreateBoxDesignerContentSp(__instance, __instance.BxOut, designer);

                __instance.BxOut.endTab(true, true);
                __instance.OTab["MP"] = designer;
                
            }
            __instance.fineTabVisibility("MAIN", false, true);  // idk but it fixes some visual things
        }
    }
}
