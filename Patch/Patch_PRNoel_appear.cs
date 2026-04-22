using HarmonyLib;
using m2d;
using nel;
using WeNeedMoreNoels.CSNetworking;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PRNoel), nameof(PRNoel.appear))]
    public class Patch_PRNoel_appear
    {
        static bool Inited;

        static bool InitedParty;

        [HarmonyPostfix]
        static void Postfix(Map2d Mp)
        {
            ShadowNoelExtensions.DisableAllShadowNoels();
            ShadowNoelExtensions.DetectShadowNoelInCurrentMap();

            if (!Inited && DB.InitConfig is not null)
            {
                WNMNTools.InitNetworking(DB.InitConfig);
                Inited = true;
            }
            if (!InitedParty)
            {
                InitedParty = true;
                PartyManager.Party party = PartyManager.InitNewParty();
                DB.partyInfos.Add(party.ID, party);
                DB.LocalNoelParty = party.ID;
            }
        }
    }
}
