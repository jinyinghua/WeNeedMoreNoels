using UnityEngine;

namespace WeNeedMoreNoels
{
    public class PartyManager
    {
        static int _unique_ID = -1;

        public static Party InitNewParty()
        {
            _unique_ID++;
            return new()
            {
                ID = _unique_ID,
                Color = Random.ColorHSV(),
                Name = $"NoelParty#{_unique_ID}"
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
