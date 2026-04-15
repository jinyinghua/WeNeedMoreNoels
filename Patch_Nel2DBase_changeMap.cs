using HarmonyLib;
using m2d;
using nel;
using UnityEngine;

namespace WeNeedMoreNoels
{
    [HarmonyPatch(typeof(NelM2DBase), nameof(NelM2DBase.changeMap), [typeof(Map2d)], [ArgumentType.Normal])]
    public class Patch_Nel2DBase_changeMap
    {
        static void Prefix()
        {
            //foreach (ShadowNoel noel in DB.shadowNoels)
            //{
            //    Object.DestroyImmediate(noel.gameObject);
            //}
            //DB.shadowNoels.Clear();
        }
    }
}
