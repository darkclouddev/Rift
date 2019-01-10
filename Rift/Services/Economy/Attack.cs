using System.Collections.Generic;

using IonicLib.Extensions;

namespace Rift.Services.Economy
{
    public class Attack
    {
        readonly ulong AttackerId;
        readonly ulong TargetId;

        readonly uint TargetCoins;
        readonly uint TargetChests;

        public SkillResult Skill;
        public AttackLoot Loot;
        public uint Count;

        static readonly List<SkillResult> CoinsSkills = new List<SkillResult>
        {
            SkillResult.GhostCoins,
            SkillResult.FlashCoins,
            SkillResult.IgniteCoins,
        };

        static readonly List<SkillResult> ChestsSkills = new List<SkillResult>
        {
            SkillResult.GhostChests,
            SkillResult.FlashChests,
            SkillResult.IgniteChests,
        };

        static readonly List<SkillResult> MuteSkills = new List<SkillResult>
        {
            SkillResult.GhostMute,
            SkillResult.FlashMute,
            SkillResult.IgniteMute,
        };

        static readonly List<SkillResult> ReversedMuteSkills = new List<SkillResult>
        {
            SkillResult.BarrierReversedMute,
            SkillResult.HealReversedMute,
        };

        static readonly List<SkillResult> NothingSkills = new List<SkillResult>
        {
            SkillResult.GhostNothing,
            SkillResult.FlashNothing,
            SkillResult.HealNothing,
            SkillResult.BarrierNothing,
        };

        public Attack(ulong attackerId, ulong targetId, uint targetCoins, uint targetChests)
        {
            AttackerId = attackerId;
            TargetId = targetId;
            TargetCoins = targetCoins;
            TargetChests = targetChests;

            FillData();
        }

        static AttackLoot GetAttackLoot(List<(uint, AttackLoot)> loots)
        {
            var curChance = Helper.NextUInt(0, 101);
            uint chance = 0;
            foreach (var item in loots)
            {
                chance += item.Item1;
                if (curChance <= chance)
                    return item.Item2;
            }

            return loots[0].Item2;
        }

        void FillData()
        {
            List<(uint, AttackLoot)> chancedLoot = new List<(uint, AttackLoot)>
            {
                (3, AttackLoot.Mute),
                (5, AttackLoot.Nothing),
                (1, AttackLoot.ReversedMute),
                (1, AttackLoot.MutualMute),
            };

            if (TargetChests >= 6)
                chancedLoot.Add((10, AttackLoot.Chests));

            if (TargetCoins >= 4000)
                chancedLoot.Add((100, AttackLoot.Coins));

            Loot = GetAttackLoot(chancedLoot);

            switch (Loot)
            {
                case AttackLoot.Chests:
                    Count = 1;
                    Skill = ChestsSkills.Random();
                    break;

                case AttackLoot.Coins:
                    Count = Helper.NextUInt(100, 601);
                    Skill = CoinsSkills.Random();
                    break;

                case AttackLoot.Mute:
                    Count = Helper.NextUInt(2, 7);
                    Skill = MuteSkills.Random();
                    break;
                case AttackLoot.MutualMute:
                    Count = Helper.NextUInt(2, 7);
                    Skill = SkillResult.IgniteMutualMute;
                    break;

                case AttackLoot.Nothing:
                    Count = 0;
                    Skill = NothingSkills.Random();
                    break;

                case AttackLoot.ReversedMute:
                    Count = Helper.NextUInt(2, 7);
                    Skill = ReversedMuteSkills.Random();
                    break;
            }
        }
    }

    public enum SkillResult
    {
        GhostCoins,
        GhostChests,
        GhostMute,
        GhostNothing,

        FlashCoins,
        FlashChests,
        FlashMute,
        FlashNothing,

        IgniteCoins,
        IgniteChests,
        IgniteMute,
        IgniteMutualMute,

        HealNothing,
        HealReversedMute,

        BarrierNothing,
        BarrierReversedMute,
    }

    public enum AttackLoot
    {
        Nothing,
        Coins,
        Chests,
        Mute,
        ReversedMute,
        MutualMute,
    }
}
