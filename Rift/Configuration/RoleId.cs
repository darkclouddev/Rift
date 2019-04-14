using System.Collections.Generic;
using System.Reflection;

using Rift.Attributes;

namespace Rift.Configuration
{
    public class RoleId
    {
        public Dictionary<string, string> GetNames()
        {
            var dict = new Dictionary<string, string>();

            var props = GetType().GetProperties();

            foreach (var prop in props)
            {
                var customAttributes = prop.GetCustomAttributes(true);

                foreach (var attribute in customAttributes)
                {
                    if (attribute is ResolveRoleAttribute resolvedAttribute)
                    {
                        dict.Add(prop.Name, resolvedAttribute.Name);
                    }
                }
            }

            return dict;
        }

        public void SetValue(string fieldName, ulong value)
        {
            var prop = GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(this, value);
            }
        }

        [ResolveRole("Чемпионы")]
        public ulong Champions { get; set; }

        [ResolveRole("Мифические")]
        public ulong Mythic { get; set; }

        [ResolveRole("Тусовые")]
        public ulong Party { get; set; }

        [ResolveRole("Отряд \"Омега\"")]
        public ulong OmegaSquad { get; set; }

        [ResolveRole("Звездные защитники")]
        public ulong StarGuardians { get; set; }

        [ResolveRole("Хекстековые")]
        public ulong Hextech { get; set; }

        [ResolveRole("Разрушители миров")]
        public ulong Worldbreakers { get; set; }

        [ResolveRole("Победоносные")]
        public ulong Victorious { get; set; }

        [ResolveRole("Вандалы")]
        public ulong Vandals { get; set; }

        [ResolveRole("Галантные")]
        public ulong Debonairs { get; set; }

        [ResolveRole("Аркадные")]
        public ulong Arcade { get; set; }

        [ResolveRole("Повелители грома")]
        public ulong ThunderLords { get; set; }

        [ResolveRole("Юстициары")]
        public ulong Justicars { get; set; }

        [ResolveRole("Кровавая луна")]
        public ulong BloodMoon { get; set; }

        [ResolveRole("Эпические")]
        public ulong Epic { get; set; }

        [ResolveRole("Исследователи ущелья")]
        public ulong RiftExplorers { get; set; }

        [ResolveRole("Охотники")]
        public ulong Hunters { get; set; }

        [ResolveRole("Ковбои")]
        public ulong Cowboys { get; set; }

        [ResolveRole("Восхождение")]
        public ulong Ascention { get; set; }

        [ResolveRole("Повелители бронзы")]
        public ulong BronzeOverlords { get; set; }

        [ResolveRole("Стримеры")]
        public ulong Streamer { get; set; }

        [ResolveRole("Модераторы")]
        public ulong Moderator { get; set; }

        [ResolveRole("Гл. Модераторы")]
        public ulong BossModerator { get; set; }

        [ResolveRole("Абсолютные")]
        public ulong Absolute { get; set; }

        [ResolveRole("Архивные")]
        public ulong Archive { get; set; }

        [ResolveRole("Блокировка чата")]
        public ulong Muted { get; set; }

        [ResolveRole("Косплееры")]
        public ulong Cosplayer { get; set; }

        [ResolveRole("Офицеры клубов")]
        public ulong ClubOfficer { get; set; }

        [ResolveRole("Хранители жетонов")]
        public ulong Keepers { get; set; }

        [ResolveRole("Призыватель атакован")]
        public ulong Attacked { get; set; }

        [ResolveRole("ХАСАГИ")]
        public ulong Hasagi { get; set; }

        [ResolveRole("Довольные поро")]
        public ulong HappyPoro { get; set; }

        [ResolveRole("Храбрые поро")]
        public ulong BravePoro { get; set; }

        [ResolveRole("Избранные")]
        public ulong Chosen { get; set; }

        [ResolveRole("Легендарные")]
        public ulong Legendary { get; set; }

        [ResolveRole("ЧОТЫРЕ")]
        public ulong Four { get; set; }

        [ResolveRole("PENTAKILL")]
        public ulong Pentakill { get; set; }

        [ResolveRole("Ранг: Железо")]
        public ulong RankIron { get; set; }

        [ResolveRole("Ранг: Бронза")]
        public ulong RankBronze { get; set; }

        [ResolveRole("Ранг: Серебро")]
        public ulong RankSilver { get; set; }

        [ResolveRole("Ранг: Золото")]
        public ulong RankGold { get; set; }

        [ResolveRole("Ранг: Платина")]
        public ulong RankPlatinum { get; set; }

        [ResolveRole("Ранг: Алмаз")]
        public ulong RankDiamond { get; set; }

        [ResolveRole("Ранг: Мастер")]
        public ulong RankMaster { get; set; }

        [ResolveRole("Ранг: Грандмастер")]
        public ulong RankGrandmaster { get; set; }

        [ResolveRole("Ранг: Претендент")]
        public ulong RankChallenger { get; set; }

        [ResolveRole("Клуб LOLRU")]
        public ulong ClubLolru { get; set; }

        [ResolveRole("Клуб Off™")]
        public ulong ClubOffTm { get; set; }

        [ResolveRole("Клуб (Octo)")]
        public ulong ClubOcto { get; set; }

        [ResolveRole("Убийца Барона")]
        public ulong BaronKiller { get; set; }

        [ResolveRole("Убийца Дракона")]
        public ulong DrakeKiller { get; set; }

        [ResolveRole("Алмазные")]
        public ulong DonateDiamond { get; set; }

        [ResolveRole("Светоносные")]
        public ulong Arclight { get; set; }

        [ResolveRole("Темная звезда")]
        public ulong DarkStar { get; set; }

        [ResolveRole("Активные")]
        public ulong Active { get; set; }

        [ResolveRole("Вардилочка")]
        public ulong Ward { get; set; }

        [ResolveRole("Реворкнутый")]
        public ulong Reworked { get; set; }

        [ResolveRole("Метовый")]
        public ulong Meta { get; set; }

        [ResolveRole("Ясуоплееры")]
        public ulong YasuoPlayer { get; set; }

        [ResolveRole("Зануда")]
        public ulong PrivateBore { get; set; }

        [ResolveRole("КАИН")]
        public ulong PrivateKayn { get; set; }

        [ResolveRole("Сэмпай")]
        public ulong PrivateSempai { get; set; }

        [ResolveRole("Mellifluous")]
        public ulong PrivateMellifluous { get; set; }

        [ResolveRole("Toxic")]
        public ulong PrivateToxic { get; set; }

        [ResolveRole("Мать осьминогов")]
        public ulong PrivateOctopusMom { get; set; }

        [ResolveRole("Токсичные")]
        public ulong Toxic { get; set; }

        [ResolveRole("Вардилочки")]
        public ulong Wardhole { get; set; }

        [ResolveRole("Престижные")]
        public ulong Prestige { get; set; }

        [ResolveRole("K/DA")]
        public ulong KDA { get; set; }
    }
}
