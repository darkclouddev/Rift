using System;

using Rift.Configuration;
using Rift.Services.Economy;

using IonicLib.Extensions;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    class AttackEmbeds
    {
        public static readonly Embed AttacksUnlockedDM =
            new EmbedBuilder()
                .WithDescription($"Призыватель, вам открыт доступ к атакам в чате.")
                .Build();

        public static readonly Embed Help =
            new EmbedBuilder()
                .WithAuthor($"Атаки")
                .WithDescription($"Нападение на призывателей осуществляется в общем чате.\n"
                                 + $"Можно воровать от 100 до 600 монет у призывателей. Есть возможность воровать сундуки из инвентаря игрока на сервере. Иногда атака может не срабатывать. Бывают и редкие атаки с блокировкой чата от 4 до 10 мин.")
                .AddField("Команда для атаки",
                          $"Напишите в общий чат {Settings.Emote.Attack} `!атаковать` и никнейм пользователя. ")
                .AddField("Стоимость атаки",
                          $"Потратьте {Settings.Emote.Coin} {Settings.Economy.AttackPrice.ToString()} монет и атакуйте призывателя в чате.")
                .WithFooter("Максимум одна атака в два часа.")
                .Build();

        public static Embed AttacksUnlockedChat(ulong userId)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель <@{userId.ToString()}> открыл доступ к атакам в чате.")
                   .Build();
        }

        public static readonly Embed SelfAttack =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Вы не можете атаковать себя.")
                .Build();

        public static readonly Embed LowLevel =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Атаковать призывателей можно лишь со {Settings.Economy.AttackMinimumLevel.ToString()} уровня.")
                .Build();

        public static readonly Embed LowTargetLevel =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Вы не можете атаковать призывателя ниже {Settings.Economy.AttackMinimumLevel.ToString()} уровня.")
                .Build();

        public static readonly Embed NoCoins =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("У вас не хватает монет для атаки.")
                .Build();

        public static readonly Embed Moderator =
            new EmbedBuilder()
                .WithDescription("Вы не можете атаковать модераторов сервера.")
                .Build();

        public static readonly Embed BotKeeper =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Нельзя атаковать хранителя ботов.")
                .Build();

        public static Embed Cooldown(TimeSpan remainingTime)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Невозможно атаковать, попробуйте через {remainingTime.FormatTimeToString()}")
                   .Build();
        }

        public static Embed TargetCooldown(TimeSpan remainingTime)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Именно этого призывателя нельзя атаковать еще {remainingTime.FormatTimeToString()}")
                   .Build();
        }

        public static Embed Chat(SocketGuildUser sgAttacker, SocketGuildUser sgTarget)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {sgAttacker.Mention} атаковал {sgTarget.Mention}.")
                   .Build();
        }

        public static Embed AttackDesc(SocketGuildUser sgAttacker, SocketGuildUser sgTarget, Attack attack)
        {
            return new EmbedBuilder()
                   .WithDescription(GetChatDescription(sgAttacker, sgTarget, attack))
                   .Build();
        }

        public static Embed Attacker(SocketGuildUser sgTarget, Attack attack)
        {
            return new EmbedBuilder()
                   .WithDescription(GetAttackerDescription(sgTarget, attack))
                   .Build();
        }

        public static Embed Target(SocketGuildUser sgAttacker, Attack attack)
        {
            return new EmbedBuilder()
                   .WithDescription(GetTargetDescription(sgAttacker, attack))
                   .Build();
        }

        static string GetChatDescription(SocketGuildUser sgAttacker, SocketGuildUser sgTarget, Attack attack)
        {
            switch (attack.Skill)
            {
                case SkillResult.GhostCoins:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Ghost} заклинание.\n"
                           + "Призыватель ворует монеты при атаке.";
                case SkillResult.GhostChests:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Ghost} заклинание.\n"
                           + "Призыватель ворует сундук при атаке.";
                case SkillResult.GhostMute:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Ghost} заклинание.\n"
                           + $"Призыватель выдает блокировку чата при атаке на {attack.Count.ToString()} мин.";
                case SkillResult.GhostNothing:
                    return $"{sgTarget.Mention} использует {Settings.Emote.Ghost} заклинание и убегает.\n"
                           + "Атакующий теряет свои монеты, атака не срабатывает.";
                case SkillResult.FlashCoins:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Flash} заклинание.\n"
                           + "Призыватель ворует монеты при атаке.";
                case SkillResult.FlashChests:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Flash} заклинание.\n"
                           + "Призыватель ворует сундук при атаке.";
                case SkillResult.FlashMute:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Flash} заклинание.\n"
                           + $"Призыватель выдает блокировку чата при атаке на {attack.Count.ToString()} мин.";
                case SkillResult.FlashNothing:
                    return $"{sgTarget.Mention} использует {Settings.Emote.Flash} заклинание и убегает.\n"
                           + "Атака не срабатывает, атакующий пытался догнать и флешнулся в стену.";
                case SkillResult.IgniteCoins:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Ignite} заклинание.\n"
                           + "Призыватель ворует монеты при атаке.";
                case SkillResult.IgniteChests:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Ignite} заклинание.\n"
                           + "Призыватель ворует сундук при атаке.";
                case SkillResult.IgniteMute:
                    return $"{sgAttacker.Mention} использует {Settings.Emote.Ignite} заклинание.\n"
                           + $"Призыватель выдает блокировку чата при атаке на {attack.Count.ToString()} мин.";
                case SkillResult.IgniteMutualMute:
                    return $"{sgAttacker.Mention} и {sgTarget.Mention} используют {Settings.Emote.Ignite} заклинания.\n"
                           + $"Игрокам выдается двойная блокировка чата на {attack.Count.ToString()} мин.";
                case SkillResult.HealNothing:
                    return $"{sgTarget.Mention} использует {Settings.Emote.Heal} заклинание.\n"
                           + "Атакующий призыватель теряет монеты, атака не срабатывает.";
                case SkillResult.HealReversedMute:
                    return $"{sgTarget.Mention} использует {Settings.Emote.Heal} заклинание.\n"
                           + $"Призыватель выдает блокировку чата атакующему на {attack.Count.ToString()} мин.";
                case SkillResult.BarrierNothing:
                    return $"{sgTarget.Mention} использует {Settings.Emote.Barrier} заклинание.\n"
                           + "Атакующий призыватель теряет монеты, атака не срабатывает.";
                case SkillResult.BarrierReversedMute:
                    return $"{sgTarget.Mention} использует {Settings.Emote.Barrier} заклинание.\n"
                           + $"Призыватель выдает блокировку чата атакующему на {attack.Count.ToString()} мин.";
                default: return "";
            }
        }

        static string GetAttackerDescription(SocketGuildUser sgTarget, Attack attack)
        {
            switch (attack.Loot)
            {
                case AttackLoot.Nothing: return $"Атака на {sgTarget.Username} не сработала, вы потеряли свои монеты.";

                case AttackLoot.Coins:
                    return $"Вы атаковали {sgTarget.Username} и украли {Settings.Emote.Coin} {attack.Count.ToString()}";

                case AttackLoot.Chests:
                    return $"Вы атаковали {sgTarget.Username} и украли {Settings.Emote.Chest} {attack.Count.ToString()}";

                case AttackLoot.Mute: return $"Вы атаковали {sgTarget.Username} и выдали блокировку чата.";

                case AttackLoot.ReversedMute:
                    return $"Вы атаковали {sgTarget.Username} и получили обратную блокировку чата.";

                case AttackLoot.MutualMute: return "Двойное заклинание, две блокировки чата.";

                default: return "";
            }
        }

        static string GetTargetDescription(SocketGuildUser sgAttacker, Attack attack)
        {
            switch (attack.Loot)
            {
                case AttackLoot.Nothing:
                    return $"Сработала защита, {sgAttacker.Username} не смог вас атаковать.";

                case AttackLoot.Coins:
                    return $"Призыватель {sgAttacker.Username} украл у вас {Settings.Emote.Coin} {attack.Count.ToString()} при атаке.";

                case AttackLoot.Chests:
                    return $"Призыватель {sgAttacker.Username} украл у вас {Settings.Emote.Chest} {attack.Count.ToString()} при атаке.";

                case AttackLoot.Mute:
                    return $"Призыватель {sgAttacker.Username} выдал вам блокировку чата при атаке.";

                case AttackLoot.ReversedMute:
                    return $"Сработала защита, вы выдали обратную блокировку {sgAttacker.Username}.";

                case AttackLoot.MutualMute:
                    return "Двойное заклинание, две блокировки чата.";

                default: return "";
            }
        }
    }
}
