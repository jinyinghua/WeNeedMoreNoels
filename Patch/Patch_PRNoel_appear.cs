using HarmonyLib;
using m2d;
using nel;
using WeNeedMoreNoels.SN;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PRNoel), nameof(PRNoel.appear))]
    public class Patch_PRNoel_appear
    {
        static bool Inited;

        [HarmonyPostfix]
        static void Postfix(Map2d Mp)
        {
            if (!Inited && DB.InitConfig is not null)
            {
                WNMNTools.InitNetworking(DB.InitConfig);
                Inited = true;
            }
            if (DB.InitConfig is not null && WNMNTools.LocalID != -1)
            {
                if (DB.InitConfig.InvisibleNickname)
                {
                    ShadowNoelExtensions.GenerateMainPRNickname(TX.Get("multiplayer_noel_nickname") + WNMNTools.LocalID.ToString());
                }
                else
                {
                    ShadowNoelExtensions.GenerateMainPRNickname(DB.InitConfig.nickName == "" ? $"Nickname#{WNMNTools.LocalID}" : DB.InitConfig.nickName);
                }
            }
        }
    }
}
