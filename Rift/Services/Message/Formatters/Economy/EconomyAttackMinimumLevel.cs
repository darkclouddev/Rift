using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class EconomyAttackMinimumLevel : FormatterBase
    {
        public EconomyAttackMinimumLevel() : base("$economyAttackMinimumLevel") {}

        public override Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, Settings.Economy.AttackMinimumLevel.ToString());
        }
    }
}
