using System;

using IonicLib.Extensions;

namespace Rift.Rewards
{
    public class Loot
    {
        public uint Count = 0;

        readonly int MinCount = 0;
        readonly int MaxCount = -1;
        readonly float Chance;

        public Loot(uint count, float chance = 100f)
        {
            Count = count;
            Chance = chance;
        }

        public Loot(int minCount, int maxCount, float chance = 100f)
        {
            MinCount = minCount;
            MaxCount = maxCount;
            Chance = chance;
        }

        public Loot CalculateLoot()
        {
            if (MaxCount != -1) // Not Fixed Count
                Count = Helper.NextUInt(MinCount, MaxCount + 1);

            return CalculateChance();
        }

        Loot CalculateChance()
        {
            if (Helper.GetPreciseChance(Chance))
                return new Loot(Count);

            return new Loot(0);
        }
    }
}
