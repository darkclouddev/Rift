using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragQueue : TemplateBase
    {
        public BragQueue() : base(nameof(BragQueue))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Brag.QueueName);
        }
    }
}
