using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Message
{
    public class MessageLink : TemplateBase
    {
        public MessageLink() : base(nameof(MessageLink))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.MessageData.Link);
        }
    }
}
