using Fusion;

namespace WeNeedMoreNoels.Networking
{
    public class ShadowNoelSpawner : SimulationBehaviour, IPlayerJoined
    {
        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                //DB.players.Add(player);
                //ShadowNoel noel = ShadowNoelExtensions.GenerateShadowNoel();
                //DB.player2NoelDic.Add(player.PlayerId, noel);
            }
        }
    }
}
