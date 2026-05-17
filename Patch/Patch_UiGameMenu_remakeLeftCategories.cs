using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel.gm;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiGameMenu), "remakeLeftCategories")]
    public class Patch_UiGameMenu_remakeLeftCategories
    {
        [HarmonyPostfix]
        private static void Postfix(UiGameMenu __instance)
        {
            Designer bxCategory = __instance.BxCategory;
            DsnDataButton dsn = new()
            {
                name = "categ_10",
                skin = "ui_category",
                skin_title = TX.Get("multiplayer_cata"),
                w = bxCategory.use_w,
                h = (bxCategory.h - bxCategory.margin_in_tb) / 11f - 8f,
                hover_to_select = true,
                fnClick = B =>
                {
                    UiMenuMul.IsMulCata = true;
                    __instance.initCategoryEdit((CATEG)10, true);
                    UiMenuMul.SendMsgButton.Select(true);
                    return true;
                },
                fnOut = B =>
                {
                    __instance.fnOutCategory(B);
                    return true;
                },
                fnHover = B => {
                    __instance.appearCategory((CATEG)10);
                    return true;
                }
            };
            bxCategory.addButton(dsn);  
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher codeMatcher = new(instructions, null);
            codeMatcher.MatchStartForward(
            [
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Sub),
                new CodeMatch(OpCodes.Ldc_R4)
            ])
                .Advance(3)
                .SetOperandAndAdvance(11f);
            return codeMatcher.Instructions();
        }
    }
}
