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
                    if (attribute is ResolveRoleAttribute resolvedAttribute)
                        dict.Add(prop.Name, resolvedAttribute.Name);
            }

            return dict;
        }

        public void SetValue(string fieldName, ulong value)
        {
            var prop = GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite) prop.SetValue(this, value);
        }

        [ResolveRole("Импульсные")] public ulong Impulse { get; set; }

        [ResolveRole("Инфернальные")] public ulong Infernal { get; set; }

        [ResolveRole("Хрустальные")] public ulong Crystal { get; set; }

        [ResolveRole("Мародеры")] public ulong Marauders { get; set; }

        [ResolveRole("Тусовые")] public ulong Party { get; set; }

        [ResolveRole("Звездные защитники")] public ulong StarGuardians { get; set; }

        [ResolveRole("Галантные")] public ulong Debonairs { get; set; }

        [ResolveRole("Аркадные")] public ulong Arcade { get; set; }

        [ResolveRole("Юстициары")] public ulong Justicars { get; set; }

        [ResolveRole("Кровавая луна")] public ulong BloodMoon { get; set; }

        [ResolveRole("Охотники")] public ulong Hunters { get; set; }

        [ResolveRole("Восхождение")] public ulong Ascention { get; set; }

        [ResolveRole("Стримеры")] public ulong Streamer { get; set; }

        [ResolveRole("Модераторы")] public ulong Moderator { get; set; }

        [ResolveRole("Абсолютные")] public ulong Absolute { get; set; }

        [ResolveRole("Блокировка чата")] public ulong Muted { get; set; }

        [ResolveRole("Легендарные")] public ulong Legendary { get; set; }

        [ResolveRole("Ранг: Железо")] public ulong RankIron { get; set; }

        [ResolveRole("Ранг: Бронза")] public ulong RankBronze { get; set; }

        [ResolveRole("Ранг: Серебро")] public ulong RankSilver { get; set; }

        [ResolveRole("Ранг: Золото")] public ulong RankGold { get; set; }

        [ResolveRole("Ранг: Платина")] public ulong RankPlatinum { get; set; }

        [ResolveRole("Ранг: Алмаз")] public ulong RankDiamond { get; set; }

        [ResolveRole("Ранг: Мастер")] public ulong RankMaster { get; set; }

        [ResolveRole("Ранг: Грандмастер")] public ulong RankGrandmaster { get; set; }

        [ResolveRole("Ранг: Претендент")] public ulong RankChallenger { get; set; }

        [ResolveRole("Темная звезда")] public ulong DarkStar { get; set; }

        [ResolveRole("Активные")] public ulong Active { get; set; }

        [ResolveRole("Зануда")] public ulong PrivateBore { get; set; }

        [ResolveRole("КАИН")] public ulong PrivateKayn { get; set; }

        [ResolveRole("Сэмпай")] public ulong PrivateSempai { get; set; }

        [ResolveRole("Mellifluous")] public ulong PrivateMellifluous { get; set; }

        [ResolveRole("Toxic")] public ulong PrivateToxic { get; set; }

        [ResolveRole("Мать осьминогов")] public ulong PrivateOctopusMom { get; set; }

        [ResolveRole("Токсичные")] public ulong Toxic { get; set; }

        [ResolveRole("Вардилочки")] public ulong Wardhole { get; set; }

        [ResolveRole("K/DA")] public ulong KDA { get; set; }

        [ResolveRole("Хранители ботов")] public ulong BotKeepers { get; set; }

        [ResolveRole("Nitro Booster")] public ulong NitroBooster { get; set; }
    }
}
