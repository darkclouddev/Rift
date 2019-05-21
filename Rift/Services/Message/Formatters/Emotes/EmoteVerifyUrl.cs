using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Emotes
{
    public class EmoteVerifyUrl : FormatterBase
    {
        public EmoteVerifyUrl() : base("$emoteVerifyUrl") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, Settings.Emote.VerifyUrl);
        }
    }
}
