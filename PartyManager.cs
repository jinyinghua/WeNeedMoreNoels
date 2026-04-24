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
            public ColorIns Color;
            public string Name;
        }
    }

    public struct ColorIns(float r, float g, float b, float a)
    {
        public float r = r;
        public float g = g;
        public float b = b;
        public float a = a;

        public static implicit operator ColorIns(Color ins)
        {
            return new(ins.r, ins.g, ins.b, ins.a);
        }

        public static implicit operator Color(ColorIns ins)
        {
            return new(ins.r, ins.g, ins.b, ins.a);
        }
    }
}
