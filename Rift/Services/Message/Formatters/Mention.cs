using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public class Mention : FormatterBase
    {
        public Mention() : base("$mention") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<@{data.UserId.ToString()}>");
        }
    }
}
