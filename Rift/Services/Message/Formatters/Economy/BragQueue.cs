using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragQueue : FormatterBase
    {
        public BragQueue() : base("$bragQueue") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.QueueName);
        }
    }
}
