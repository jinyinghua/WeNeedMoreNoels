using System.Collections.Generic;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.Networking.ReceiveEvent;

namespace WeNeedMoreNoels.Networking
{
    public static class ReceiveMessageManager
    {
        static List<PeerReceiveMessageBase> list = [];

        static List<bool> debugEnabledList = [];

        public static void Init()
        {
            RegisterReceiveMessage(new InitNoelEvent());
            RegisterReceiveMessage(new UpdateNoelInfoEvent());
            RegisterReceiveMessage(new NotifyNoelDamageEvent());
            RegisterReceiveMessage(new NotifyNoelMagicEvent());
            RegisterReceiveMessage(new NotifyNoelStartBattleEvent());
            RegisterReceiveMessage(new NotifyNoelEndBattleEvent());
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
