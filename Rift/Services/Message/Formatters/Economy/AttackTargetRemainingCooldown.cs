using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Economy
{
    public class AttackTargetRemainingCooldown : FormatterBase
    {
        public AttackTargetRemainingCooldown() : base("$attackTargetRemainingCooldown") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cds = await Database.GetUserCooldownsAsync(data.AttackUserId);

            return await ReplaceData(message, cds.BeingAttackedTimeSpan.Humanize());
        }
    }
}
