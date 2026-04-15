using m2d;
using System.Collections;
using UnityEngine;
using XX;

namespace WeNeedMoreNoels
{
    public static class ShadowNoelExtensions
    {
        public static ShadowNoel GenerateShadowNoel(int id = -1)
        {
            Map2d map = M2DBase.Instance.curMap;
            map.Pr.getPosition(out float x, out float y);
            ShadowNoel noel = map.createMover<ShadowNoel>("ShadowNoel", x, y);
            noel.newGame();
            noel.gameObject.AddComponent<Rigidbody2D>();
            noel.gameObject.name = "ShadowNoel";
            map.assignMover(noel);
            DB.noelDics.Add(id, noel);
            return noel;
        }

        public static void DestoryShadowNoel(int id)
        {
            if (!DB.noelDics.ContainsKey(id))
            {
                Plugin.Logger.LogWarning("try to remove not existing value");
            }
            Object.DestroyImmediate(DB.noelDics[id].gameObject);
            DB.noelDics.Remove(id);
        }

        public static void MoveShadowNoel(ShadowNoel noel, System.Numerics.Vector2 pos)
        {
            noel.getPosition(out float x, out float y);
            float dx = pos.X - x;
            float dy = pos.Y - y;
            noel.walkBy(FOCTYPE.WALK, dx, dy, true);
        }

        public static void WalkDummy(ShadowNoel noel, float t, AIM aim, float speed)
        {
            noel.StartCoroutine(WalkCoroutine(noel, aim, speed, t));
        }

        static IEnumerator WalkCoroutine(ShadowNoel noel, AIM aim, float speed, float targetTime)
        {
            float timer = 0;
            ShadowNoelAnimator anm = (ShadowNoelAnimator)noel.Anm;
            anm.setPose(WALK_STATE);
            while (timer < targetTime)
            {
                noel.walkByAim((int)aim, speed);
                timer += Time.deltaTime;
                yield return null;
            }
            anm.setPose(STAND_STATE);
            yield return null;
        }

        const string WALK_STATE = "walk";

        const string STAND_STATE = "stand";
    }
}
