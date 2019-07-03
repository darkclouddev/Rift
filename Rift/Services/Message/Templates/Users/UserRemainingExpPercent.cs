using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserRemainingExpPercent : TemplateBase
    {
        public UserRemainingExpPercent() : base(nameof(UserRemainingExpPercent)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var user = await DB.Users.GetAsync(data.UserId);
            var currentLevelExp = EconomyService.GetExpForLevel(user.Level);
            var fullLevelExp = EconomyService.GetExpForLevel(user.Level + 1u) - currentLevelExp;
            var remainingExp = fullLevelExp - (user.Experience - currentLevelExp);
            var levelPerc = 100 - (int)Math.Floor((float)remainingExp / fullLevelExp * 100);

            return await ReplaceData(message, levelPerc.ToString());
        }
    }
}
