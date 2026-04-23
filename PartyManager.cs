using UnityEngine;

namespace WeNeedMoreNoels
{
    public class PartyManager
    {
        public static Party InitNewParty(int id)
        {
            return new()
            {
                ID = id,
                Color = Random.ColorHSV(),
                Name = $"NoelParty#{id}"
            };
        }

        public class Party
        {
            public int ID;
            public Color Color;
            public string Name;
        }
    }
}
