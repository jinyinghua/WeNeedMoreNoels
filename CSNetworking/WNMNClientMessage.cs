using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.CSNetworking
{
    public class WNMNClientMessage
    {
        public string NickName;
        public NoelType NoelType;
        public int ID;
        public string IP;
        public int Port;
        public PartyManager.Party Party;
    }
}
