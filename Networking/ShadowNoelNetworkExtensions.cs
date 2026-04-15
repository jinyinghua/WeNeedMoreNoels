namespace WeNeedMoreNoels.Networking
{
    public static class ShadowNoelNetworkExtensions
    {
        public static void Walk(ShadowNoel noel)
        {
            noel.Anm.setPose("walk");
        }

        public static void Stand(ShadowNoel noel)
        {
            noel.Anm.setPose("stand");
        }
    }
}
