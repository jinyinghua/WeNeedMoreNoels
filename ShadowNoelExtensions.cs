using m2d;

namespace WeNeedMoreNoels
{
    public static class ShadowNoelExtensions
    {
        public static void GenerateShadowNoel()
        {
            Map2d map = M2DBase.Instance.curMap;
            map.Pr.getPosition(out float x, out float y);
            ShadowNoel noel = map.createMover<ShadowNoel>("ShadowNoel", x, y);
            noel.newGame();
            noel.gameObject.AddComponent<UnityEngine.Rigidbody2D>();
            noel.gameObject.name = "ShadowNoel";
            map.assignMover(noel);
        }
    }
}
