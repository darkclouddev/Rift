using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserRemainingExpPercent : FormatterBase
    {
        public UserRemainingExpPercent() : base("$userRemainingExpPercent") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var user = await Database.GetUserAsync(data.UserId);
            var currentLevelExp = EconomyService.GetExpForLevel(user.Level);
            var fullLevelExp = EconomyService.GetExpForLevel(user.Level + 1u) - currentLevelExp;
            var remainingExp = fullLevelExp - (user.Experience - currentLevelExp);
            var levelPerc = 100 - (int)Math.Floor((float)remainingExp / fullLevelExp * 100);

            return await ReplaceData(message, levelPerc.ToString());
        }
    }
}
