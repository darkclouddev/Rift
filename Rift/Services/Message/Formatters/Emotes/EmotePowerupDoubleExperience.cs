using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Emotes
{
    public class EmotePowerupDoubleExperience : FormatterBase
    {
        public EmotePowerupDoubleExperience() : base("$emotePowerupDoubleExperience") {}

        public override Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, Settings.Emote.PowerupDoubleExperience);
        }
    }
}
