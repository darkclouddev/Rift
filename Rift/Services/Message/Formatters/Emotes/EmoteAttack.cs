using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Emotes
{
    public class EmoteAttack : FormatterBase
    {
        public EmoteAttack() : base("$emoteAttack") {}

        public override Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, Settings.Emote.Attack);
        }
    }
}
