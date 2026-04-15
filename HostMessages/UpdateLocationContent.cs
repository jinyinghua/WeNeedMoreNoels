using System.Collections.Generic;
using System.Numerics;

namespace WeNeedMoreNoels.HostMessages
{
    public class UpdateLocationContent
    {
        public Vector2 HostPosition;

        public List<KeyValuePair<int, Vector2>> PeerPositions;
    }
}
