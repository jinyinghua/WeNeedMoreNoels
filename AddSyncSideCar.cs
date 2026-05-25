using nel;
using UnityEngine;

namespace WeNeedMoreNoels
{
    public class AddSyncSideCar : MonoBehaviour
    {
        public int syncID;
        public int peerID;

        void Update()
        {
            NelEnemyBoss boss = FindObjectOfType<NelEnemyBoss>();
            if (boss == null)
            {
                return;
            }
            DB.CurEnemies.Add(boss);
            EnemySynchronizerSyncClient client = boss.gameObject.AddComponent<EnemySynchronizerBossClient>();
            client.SyncID = syncID;
            client.PeerID = peerID;
            DB.SyncClients.Add(syncID, client);
            if (!DB.peerClients.ContainsKey(peerID))
            {
                DB.peerClients.Add(peerID, []);
            }
            DB.peerClients[peerID].Add(client);
            Destroy(gameObject);
        }
    }
}
