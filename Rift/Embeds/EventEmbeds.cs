using Rift.Configuration;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class EventEmbeds
    {
        static readonly string helpText = "Для атаки на лесного монстра нажмите на иконку смайта.";
        static readonly string winnerText = "Один из участвующих получит дополнительную награду.";

        public static readonly Embed embedMsg =
            new EmbedBuilder()
                .WithAuthor("Лесной монстр")
                .WithDescription($"Призыватели, чат отключается для сражения.")
                .Build();

        public static Embed Baron(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Барон Нашор", Settings.Emote.BaronUrl)
                   .WithDescription($"{helpText}\n{winnerText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074046967250954/cd63fffdf1750801.png")
                   .Build();
        }

        public static Embed Drake(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Дракон", Settings.Emote.DrakeUrl)
                   .WithDescription($"{helpText}\n{winnerText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074050515501076/1fa5444f7bf27e3b.png")
                   .Build();
        }

        public static Embed Wolves(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Волки мрака", Settings.Emote.WolvesUrl)
                   .WithDescription($"{helpText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074047743197184/16db99d619a6c02e.png")
                   .Build();
        }

        public static Embed Razorfins(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Остроклювы", Settings.Emote.RazorfinsUrl)
                   .WithDescription($"{helpText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074052407263232/4baa2a6d5bd37d0a.png")
                   .Build();
        }

        public static Embed Krug(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Големы", Settings.Emote.KrugUrl)
                   .WithDescription($"{helpText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074048464486423/136f2fdf8d25a2d6.png")
                   .Build();
        }

        public static Embed RedBuff(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Красный бафф", Settings.Emote.RedBuffUrl)
                   .WithDescription($"{helpText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074051501162506/92d41f53e8bb9431.png")
                   .Build();
        }

        public static Embed BlueBuff(Reward reward)
        {
            return new EmbedBuilder()
                   .WithAuthor("Синий бафф", Settings.Emote.BlueBuffUrl)
                   .WithDescription($"{helpText}\n\n" + $"Награда за убийство: {reward.RewardString}")
                   .WithImageUrl("https://cdn.discordapp.com/attachments/372852455119257600/476074054038585365/4764815448229f02.png")
                   .Build();
        }

        public static Embed UserCount(int count)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватели убили лесного монстра и забрали награду.\n"
                                    + $"Всего в сражении участвовало **{count}** призывателей.")
                   .Build();
        }

        public static Embed Winner(ulong winnerId, string reward)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель <@{winnerId}> после сражения получил {reward}")
                   .Build();
        }
    }
}
