using System.Collections.Generic;

namespace WeNeedMoreNoels.HostMessages
{
    public class HostUpdateContent<T>
    {
        public T HostContent;

        public List<KeyValuePair<int, T>> PeerContents;
    }
}
