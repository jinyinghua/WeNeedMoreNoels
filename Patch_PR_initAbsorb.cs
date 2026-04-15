using HarmonyLib;
using m2d;
using nel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WeNeedMoreNoels
{
    [HarmonyPatch(typeof(PR), nameof(PR.initAbsorb))]
    public class Patch_PR_initAbsorb
    {
        /*ldarg.0
ldfld class nel.UIPicture nel.PR::UP
brfalse.s IL_0157*/
        //[HarmonyPatch]
        //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        //{
        //    var match = new CodeMatcher(instructions, generator);
        //    FieldInfo up = AccessTools.Field(typeof(PR), "UP");
        //    MethodInfo call = AccessTools.Method(typeof(PR), "playAwkVo");
        //    match.MatchForward(false, new CodeMatch(OpCodes.Call, call));
        //    match.Advance(-1);
        //    match.CreateLabel(out Label target);
        //    match.MatchBack(false, new CodeMatch(OpCodes.Ldfld, up));
        //    match.Advance(1);
        //    match.InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, target));
        //    match.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));
        //    match.InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, up));
        //    return match.Instructions();
        //}

        [HarmonyFinalizer]
        static Exception Finalizer(Exception __exception)
        {
            //Plugin.Logger.LogInfo(__exception.ToString());
            //Plugin.Logger.LogInfo(__exception.StackTrace);
            return null;
        }
    }
}
