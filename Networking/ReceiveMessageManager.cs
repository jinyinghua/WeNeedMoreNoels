using System.Collections.Generic;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking
{
    public static class ReceiveMessageManager
    {
        static List<PeerReceiveMessageBase> list;

        static List<bool> debugEnabledList;

        public static void Init()
        {

        }

        public static void RegisterReceiveMessage(PeerReceiveMessageBase receive)
        {
            list.Add(receive);
            debugEnabledList.Add(false);
        }

        public static IEnumerable<PeerReceiveMessageBase> GetAllReceives(WNMNPeerMessage message)
        {
            for (int i = 0; i < list.Count; i++)
            {
                PeerReceiveMessageBase receive = list[i];
                if (debugEnabledList[i])
                {
                    Plugin.Logger.LogInfo(receive.ToMessageString(message));
                }
                yield return receive;
            }
        }
    }
}
