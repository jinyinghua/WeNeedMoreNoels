using System.Collections.Generic;

namespace WeNeedMoreNoels.HostMessages
{
    public class UpdateLocationContent
    {
        public ShadowNoelLocation HostLocation;

        public List<KeyValuePair<int, ShadowNoelLocation>> PeerLocations;
    }
}
