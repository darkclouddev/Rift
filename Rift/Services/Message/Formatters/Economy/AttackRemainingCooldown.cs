using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Economy
{
    public class AttackRemainingCooldown : FormatterBase
    {
        public AttackRemainingCooldown() : base("$attackRemainingCooldown") {}

        public override async Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            var cds = await Database.GetUserCooldownsAsync(userId);

            return await ReplaceData(message, cds.AttackTimeSpan.Humanize(1));
        }
    }
}
